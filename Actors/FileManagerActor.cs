using Akka.Actor;
using Akka.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuneTweak.Actors
{
    public class FileManagerActor : ReceiveActor
    {
        private readonly List<string> _selectedFiles = new List<string>();
        private readonly HashSet<string> _supportedExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            ".mp3", ".wav", ".aac", ".flac", ".ogg", ".wma", ".m4a"
        };

        private readonly IActorRef _coordinator;
        private readonly IActorRef _metadataActor;

        private readonly ILoggingAdapter _log = Context.GetLogger();

        public FileManagerActor(IActorRef metadataActor, IActorRef coordinator)
        {
            _coordinator = coordinator;
            _metadataActor = metadataActor;

            Receive<SelectFilesMessage>(HandleSelectFiles);
            Receive<SelectFolderMessage>(HandleSelectFolder);
            Receive<GetSelectedFilesMessage>(HandleGetSelectedFiles);
            //Receive<ClearSelectedFilesMessage>(HandleClearSelectedFiles);
        }

        private void HandleSelectFiles(SelectFilesMessage msg)
        {
            try
            {
                var validFiles = msg.Files
                    .Where(IsValidAudioFile)
                    .Where(file => !_selectedFiles.Contains(file, StringComparer.OrdinalIgnoreCase))
                    .ToList();

                if (!validFiles.Any())
                {
                    _coordinator.Tell(new UpdateStatusMessage("No valid audio files selected."));
                    return;
                }

                AddFiles(validFiles);

                _coordinator.Tell(new UpdateStatusMessage(
                    validFiles.Count == 1
                        ? $"Added 1 file. Total: {_selectedFiles.Count} files."
                        : $"Added {validFiles.Count} files. Total: {_selectedFiles.Count} files."
                ));
            }
            catch (Exception ex)
            {
                var errMsg = $"Error selecting files: {ex.Message}";
                _log.Error(ex, errMsg);
                _coordinator.Tell(new UpdateStatusMessage(errMsg));
            }
        }

        private void HandleSelectFolder(SelectFolderMessage msg)
        {
            try
            {
                if (!Directory.Exists(msg.FolderPath))
                {
                    _coordinator.Tell(new UpdateStatusMessage($"Folder not found: {msg.FolderPath}"));
                    return;
                }

                var searchOption = msg.Recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

                // Single pass: scan all files, then filter by extension
                IEnumerable<string> allFiles;
                try
                {
                    allFiles = Directory.EnumerateFiles(msg.FolderPath, "*.*", searchOption);
                }
                catch (Exception ex) when (ex is UnauthorizedAccessException or DirectoryNotFoundException)
                {
                    _log.Warning("Error accessing folder {0}: {1}", msg.FolderPath, ex.Message);
                    _coordinator.Tell(new UpdateStatusMessage($"Error accessing folder: {msg.FolderPath}"));
                    return;
                }

                var newFiles = allFiles
                    .Where(IsValidAudioFile)
                    .Where(file => !_selectedFiles.Contains(file, StringComparer.OrdinalIgnoreCase))
                    .ToList();

                if (!newFiles.Any())
                {
                    _coordinator.Tell(new UpdateStatusMessage("No new audio files found in the selected folder."));
                    return;
                }

                AddFiles(newFiles);

                var recursiveText = msg.Recursive ? " (including subfolders)" : "";
                _coordinator.Tell(new UpdateStatusMessage(
                    newFiles.Count == 1
                        ? $"Added 1 file from folder{recursiveText}. Total: {_selectedFiles.Count} files."
                        : $"Added {newFiles.Count} files from folder{recursiveText}. Total: {_selectedFiles.Count} files."
                ));
            }
            catch (Exception ex)
            {
                var errMsg = $"Error scanning folder: {ex.Message}";
                _log.Error(ex, errMsg);
                _coordinator.Tell(new UpdateStatusMessage(errMsg));
            }
        }

        private void HandleGetSelectedFiles(GetSelectedFilesMessage msg)
        {
            // Return a copy to prevent external modification
            Sender.Tell(new List<string>(_selectedFiles));
        }

        private void AddFiles(List<string> files)
        {
            foreach (var file in files)
            {
                _selectedFiles.Add(file);
            }

            // Batch metadata requests instead of hammering MetadataActor
            foreach (var file in files)
            {
                _metadataActor.Tell(new ReadMetadataMessage(file));
            }
        }

        //private void HandleClearSelectedFiles(ClearSelectedFilesMessage msg)
        //{
        //    var count = _selectedFiles.Count;
        //    _selectedFiles.Clear();
        //    _coordinator.Tell(new UpdateStatusMessage($"Cleared {count} selected files."));
        //}

        private bool IsValidAudioFile(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                return false;

            try
            {
                if (!File.Exists(filePath))
                    return false;

                var extension = Path.GetExtension(filePath);
                return _supportedExtensions.Contains(extension);
            }
            catch
            {
                return false;
            }
        }
    }
}
