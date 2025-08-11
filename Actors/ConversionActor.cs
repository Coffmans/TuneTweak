using Akka.Actor;
using Akka.Event;
using FFMpegCore;
using FFMpegCore.Arguments;
using FFMpegCore.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TagLib.Mpeg;

namespace TuneTweak.Actors
{
    public class ConversionActor : ReceiveActor
    {
        private readonly string ffmpegPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ffmpeg.exe");
        private readonly string ffprobePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ffprobe.exe");

        private readonly IActorRef _coordinator;
        private readonly IActorRef _metadataActor;
        private readonly ILoggingAdapter _log = Context.GetLogger();

        private readonly Dictionary<string, string> _codecMap = new()
        {
            ["mp3"] = "libmp3lame",
            ["aac"] = "aac",
            ["wav"] = "pcm_s16le",
            ["ogg"] = "libvorbis",
            ["flac"] = "flac",
            ["m4a"] = "aac"
        };

        private readonly Dictionary<string, string> _qualitySettings = new()
        {
            ["mp3"] = "-b:a 320k",
            ["aac"] = "-b:a 256k",
            ["ogg"] = "-q:a 8",
            ["flac"] = "",
            ["wav"] = "",
            ["m4a"] = "-b:a 256k"
        };

        public ConversionActor(IActorRef coordinator, IActorRef metadata)
        {
            _coordinator = coordinator;
            _metadataActor = metadata;

            ReceiveAsync<ConvertFileMessage>(HandleConvertFile);

        }
        private async Task HandleConvertFile(ConvertFileMessage msg)
        {
            try
            {
                var format = msg.OutputFormat?.ToLowerInvariant();
                var validation = ValidateConversionRequest(msg.FilePath, format);

                if (!validation.isValid)
                {
                    _coordinator.Tell(new ConversionCompleteMessage(msg.FilePath, false, validation.errorMessage));
                    return;
                }

                // Avoid overwriting input
                var outputDir = GetOutputDirectory(msg.FilePath);
                var outputFile = Path.Combine(outputDir, $"{Path.GetFileNameWithoutExtension(msg.FilePath)}.{format}");
                if (string.Equals(msg.FilePath, outputFile, StringComparison.OrdinalIgnoreCase))
                {
                    _coordinator.Tell(new ConversionCompleteMessage(msg.FilePath, false, "Input and output paths are the same."));
                    return;
                }

                var metadata = await _metadataActor
                    .Ask<Dictionary<string, string>>(new ReadMetadataMessage(msg.FilePath), TimeSpan.FromSeconds(10));

                await ConvertFileAsync(msg.FilePath, format!, metadata, outputFile);
            }
            catch (Exception ex)
            {
                var errMsg = $"Conversion failed for {msg.FilePath}: {ex.Message}";
                _log.Error(ex, errMsg);
                _coordinator.Tell(new ConversionCompleteMessage(msg.FilePath, false, errMsg));
            }
        }

        private (bool isValid, string errorMessage) ValidateConversionRequest(string filePath, string? format)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                return (false, "File path is empty.");

            if (!System.IO.File.Exists(filePath))
                return (false, "File does not exist.");

            if (string.IsNullOrWhiteSpace(format) || !_codecMap.ContainsKey(format))
                return (false, $"Unsupported output format: {format}");

            if (!System.IO.File.Exists(ffmpegPath))
                return (false, "FFmpeg not found. Ensure ffmpeg.exe is in the bin folder.");

            if (!System.IO.File.Exists(ffprobePath))
                return (false, "FFprobe not found. Ensure ffprobe.exe is in the bin folder.");

            return (true, string.Empty);
        }

        private async Task ConvertFileAsync(string inputFile, string format, Dictionary<string, string> metadata, string outputFile)
        {
            var codec = _codecMap[format];
            var qualityArgs = _qualitySettings.GetValueOrDefault(format, "");

            Directory.CreateDirectory(Path.GetDirectoryName(outputFile)!);
            outputFile = GetUniqueFileName(outputFile);

            try
            {
                var duration = await GetAudioDurationAsync(inputFile);
                if (duration <= 0)
                {
                    _coordinator.Tell(new ConversionCompleteMessage(inputFile, false, "Could not determine audio duration."));
                    return;
                }

                var ffmpegArgs = FFMpegArguments.FromFileInput(inputFile);

                await ffmpegArgs
                    .OutputToFile(outputFile, overwrite: true, options =>
                    {
                        options.WithCustomArgument($"-c:a {codec}");
                        if (!string.IsNullOrEmpty(qualityArgs))
                            options.WithCustomArgument(qualityArgs);

                        options.WithCustomArgument("-map_metadata 0");

                        if (format != "wav")
                            options.WithCustomArgument("-af volume=2.0");
                    })
                    .NotifyOnProgress(progress =>
                    {
                        var percent = duration > 0 ? (progress / duration) * 100 : 0;
                        _coordinator.Tell(new ConversionProgressMessage(inputFile, (int)Math.Min(percent, 100)));
                    }, TimeSpan.FromSeconds(1))
                    .ProcessAsynchronously();

                await UpdateOutputMetadata(outputFile, metadata);

                _coordinator.Tell(new ConversionCompleteMessage(inputFile, true));
            }
            catch (Exception ex)
            {
                try { if (System.IO.File.Exists(outputFile)) System.IO.File.Delete(outputFile); } catch { }
                var errorMessage = $"FFmpeg conversion failed: {ex.Message}";
                _log.Error(ex, errorMessage);
                _coordinator.Tell(new ConversionCompleteMessage(inputFile, false, errorMessage));
            }
        }

        private string GetOutputDirectory(string inputFilePath)
        {
            var inputDir = Path.GetDirectoryName(inputFilePath);
            return Path.IsPathRooted(inputFilePath) && !string.IsNullOrEmpty(inputDir)
                ? Path.Combine(inputDir, "Converted")
                : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Converted");
        }

        private string GetUniqueFileName(string filePath)
        {
            if (!System.IO.File.Exists(filePath))
                return filePath;

            var dir = Path.GetDirectoryName(filePath)!;
            var baseName = Path.GetFileNameWithoutExtension(filePath);
            var ext = Path.GetExtension(filePath);

            for (int i = 1; i < 1000; i++)
            {
                var candidate = Path.Combine(dir, $"{baseName} ({i}){ext}");
                if (!System.IO.File.Exists(candidate))
                    return candidate;
            }

            throw new IOException("Could not generate unique filename after 1000 attempts.");
        }

        private async Task<double> GetAudioDurationAsync(string inputFile)
        {
            try
            {
                var mediaInfo = await FFProbe.AnalyseAsync(inputFile);
                return mediaInfo.Duration.TotalSeconds;
            }
            catch
            {
                // Fallback to TagLib
                try
                {
                    using var file = TagLib.File.Create(inputFile);
                    return file.Properties.Duration.TotalSeconds;
                }
                catch
                {
                    return 0;
                }
            }
        }
        private async Task UpdateOutputMetadata(string outputFile, Dictionary<string, string> metadata)
        {
            try
            {
                using var file = TagLib.File.Create(outputFile);

                if (metadata.TryGetValue("title", out var title))
                    file.Tag.Title = title;

                if (metadata.TryGetValue("artist", out var artist))
                    file.Tag.Performers = new[] { artist };

                if (metadata.TryGetValue("album", out var album))
                    file.Tag.Album = album;

                if (metadata.TryGetValue("year", out var year) && int.TryParse(year, out var yearInt))
                    file.Tag.Year = (uint)yearInt;

                if (metadata.TryGetValue("genre", out var genre))
                    file.Tag.Genres = new[] { genre };

                if (metadata.TryGetValue("track", out var track) && int.TryParse(track, out var trackInt))
                    file.Tag.Track = (uint)trackInt;

                // Copy album art if available
                if (metadata.TryGetValue("albumArtPath", out var artPath) && System.IO.File.Exists(artPath))
                {
                    var picData = System.IO.File.ReadAllBytes(artPath);
                    var picture = new TagLib.Picture(new TagLib.ByteVector(picData))
                    {
                        Type = TagLib.PictureType.FrontCover,
                        MimeType = System.Net.Mime.MediaTypeNames.Image.Jpeg
                    };
                    file.Tag.Pictures = new TagLib.IPicture[] { picture };
                }

                file.Save();
            }
            catch (Exception ex)
            {
                var warnMsg = $"Warning: Could not update metadata for {Path.GetFileName(outputFile)}: {ex.Message}";
                _log.Warning(warnMsg);
                _coordinator.Tell(new UpdateStatusMessage(warnMsg));
            }
        }
    }
}
