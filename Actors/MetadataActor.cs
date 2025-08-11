using Akka.Actor;
using Akka.Event;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuneTweak.Actors
{
    public class MetadataActor : ReceiveActor
    {
        private readonly IActorRef _coordinator;
        private readonly ILoggingAdapter _log = Context.GetLogger();
        private readonly Dictionary<string, (Dictionary<string, string> metadata, Image? albumArt)> _metadataCache = new();
        private readonly List<string> _tempArtFiles = new(); // track temp image paths for cleanup

        public MetadataActor(IActorRef coordinator)
        {
            _coordinator = coordinator;

            Receive<ReadMetadataMessage>(HandleReadMetadata);
            Receive<UpdateMetadataMessage>(HandleUpdateMetadata);

        }
        private void HandleReadMetadata(ReadMetadataMessage msg)
        {
            try
            {
                // Check cache first
                if (_metadataCache.TryGetValue(msg.FilePath, out var cached))
                {
                    Sender.Tell(cached.metadata);
                    _coordinator.Tell(new FileAddedMessage(msg.FilePath, cached.metadata, cached.albumArt!));
                    return;
                }

                var (metadata, albumArt) = ReadMetadata(msg.FilePath);

                // Cache the result
                _metadataCache[msg.FilePath] = (metadata, albumArt);

                Sender.Tell(metadata);
                _coordinator.Tell(new FileAddedMessage(msg.FilePath, metadata, albumArt!));
            }
            catch (Exception ex)
            {
                var errorMsg = $"Error reading metadata for {Path.GetFileName(msg.FilePath)}: {ex.Message}";
                _log.Error(ex, errorMsg);
                _coordinator.Tell(new UpdateStatusMessage(errorMsg));

                var emptyMetadata = new Dictionary<string, string>
                {
                    ["title"] = "",
                    ["artist"] = "",
                    ["album"] = ""
                };
                Sender.Tell(emptyMetadata);
            }
        }

        private void HandleUpdateMetadata(UpdateMetadataMessage msg)
        {
            try
            {
                UpdateMetadataFile(msg.FilePath, msg.Title, msg.Artist);

                // Update cache
                var albumArt = _metadataCache.TryGetValue(msg.FilePath, out var cached)
                    ? cached.albumArt
                    : null;

                var albumArtPath = albumArt != null ? SaveTempAlbumArt(albumArt, msg.FilePath) : string.Empty;

                var metadata = new Dictionary<string, string>
                {
                    ["title"] = msg.Title,
                    ["artist"] = msg.Artist,
                    ["album"] = _metadataCache.TryGetValue(msg.FilePath, out var existing)
                        ? existing.metadata.GetValueOrDefault("album", "")
                        : "",
                    ["albumArtPath"] = albumArtPath
                };

                _metadataCache[msg.FilePath] = (metadata, albumArt);

                _coordinator.Tell(new FileAddedMessage(msg.FilePath, metadata, albumArt!));
                _coordinator.Tell(new UpdateStatusMessage($"Metadata updated for {Path.GetFileName(msg.FilePath)}"));
            }
            catch (Exception ex)
            {
                var errMsg = $"Error updating metadata for {Path.GetFileName(msg.FilePath)}: {ex.Message}";
                _log.Error(ex, errMsg);
                _coordinator.Tell(new UpdateStatusMessage(errMsg));
            }
        }

        private (Dictionary<string, string>, Image?) ReadMetadata(string inputFile)
        {
            if (!File.Exists(inputFile))
            {
                throw new FileNotFoundException($"File not found: {inputFile}");
            }

            try
            {
                using var file = TagLib.File.Create(inputFile);

                var metadata = new Dictionary<string, string>
                {
                    ["title"] = file.Tag.Title ?? Path.GetFileNameWithoutExtension(inputFile),
                    ["artist"] = file.Tag.FirstPerformer ?? "Unknown Artist",
                    ["album"] = file.Tag.Album ?? "Unknown Album",
                    ["duration"] = file.Properties.Duration.ToString(@"mm\:ss"),
                    ["bitrate"] = file.Properties.AudioBitrate.ToString(),
                    ["format"] = Path.GetExtension(inputFile).TrimStart('.')
                };

                Image? albumArt = null;
                string albumArtPath = string.Empty;
                if (file.Tag.Pictures?.Length > 0)
                {
                    try
                    {
                        var picture = file.Tag.Pictures[0];
                        using var ms = new MemoryStream(picture.Data.Data);
                        using var originalImage = Image.FromStream(ms);

                        // Create thumbnail
                        albumArt = new Bitmap(50, 50);
                        using var g = Graphics.FromImage(albumArt);
                        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        g.SmoothingMode = SmoothingMode.HighQuality;
                        g.CompositingQuality = CompositingQuality.HighQuality;
                        g.DrawImage(originalImage, 0, 0, 50, 50);

                        albumArtPath = SaveTempAlbumArt(originalImage, inputFile);
                    }
                    catch (Exception imgEx)
                    {
                        _log.Warning("Failed to process album art for {0}: {1}", inputFile, imgEx.Message);
                    }
                }

                metadata["albumArtPath"] = albumArtPath;

                return (metadata, albumArt);
            }
            catch (Exception ex)
            {
                _log.Warning("Failed to read metadata from {0}: {1}", inputFile, ex.Message);

                return (new Dictionary<string, string>
                {
                    ["title"] = Path.GetFileNameWithoutExtension(inputFile),
                    ["artist"] = "Unknown Artist",
                    ["album"] = "Unknown Album",
                    ["duration"] = "00:00",
                    ["bitrate"] = "0",
                    ["format"] = Path.GetExtension(inputFile).TrimStart('.'),
                    ["albumArtPath"] = string.Empty
                }, null);
            }
        }

        private string SaveTempAlbumArt(Image image, string sourceFilePath)
        {
            try
            {
                var tempPath = Path.Combine(Path.GetTempPath(),
                    $"{Path.GetFileNameWithoutExtension(sourceFilePath)}_albumart.jpg");

                image.Save(tempPath, System.Drawing.Imaging.ImageFormat.Jpeg);
                _tempArtFiles.Add(tempPath);
                return tempPath;
            }
            catch (Exception ex)
            {
                _log.Warning("Failed to save temp album art for {0}: {1}", sourceFilePath, ex.Message);
                return string.Empty;
            }
        }

        private void UpdateMetadataFile(string filePath, string title, string artist)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"File not found: {filePath}");
            }

            using var file = TagLib.File.Create(filePath);
            file.Tag.Title = title;
            file.Tag.Performers = new[] { artist };
            file.Save();
        }

        protected override void PostStop()
        {
            // Dispose cached images
            foreach (var (_, albumArt) in _metadataCache.Values)
            {
                albumArt?.Dispose();
            }
            _metadataCache.Clear();

            // Delete temp album art files
            foreach (var tempFile in _tempArtFiles)
            {
                try
                {
                    if (File.Exists(tempFile))
                        File.Delete(tempFile);
                }
                catch { }
            }
            _tempArtFiles.Clear();

            base.PostStop();
        }
    }
}
