using Akka.Actor;
using Akka.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuneTweak.Interfaces;

namespace TuneTweak.Actors
{
    public class CoordinatorActor : ReceiveActor
    {
        private readonly IUIUpdater _uiUpdater;
        private readonly IActorRef _fileManagerActor;
        private readonly IActorRef _metadataActor;
        private readonly IActorRef _conversionActor;
        private readonly ILoggingAdapter _log = Context.GetLogger();

        private readonly Queue<string> _filesToConvert = new();
        private readonly Dictionary<string, int> _retryCounts = new();
        private string _currentOutputFormat = string.Empty;

        private const int MaxRetriesPerFile = 2; // configurable

        private readonly HashSet<string> _supportedFormats = new(StringComparer.OrdinalIgnoreCase)
        {
            "mp3", "wav", "aac", "flac", "ogg", "wma", "m4a"
        };

        public CoordinatorActor(IUIUpdater uiUpdater)
        {
            _uiUpdater = uiUpdater ?? throw new ArgumentNullException(nameof(uiUpdater));
            _metadataActor = Context.ActorOf(Props.Create(() => new MetadataActor(Self)), "metadata");
            _fileManagerActor = Context.ActorOf(Props.Create(() => new FileManagerActor(_metadataActor, Self)), "fileManager");
            _conversionActor = Context.ActorOf(Props.Create(() => new ConversionActor(Self, _metadataActor)), "conversion");

            RegisterMessageHandlers();
        }

        private void RegisterMessageHandlers()
        {
            Receive<SelectFilesMessage>(msg =>
            {
                _fileManagerActor.Tell(msg);
            });

            Receive<SelectFolderMessage>(msg =>
            {
                _fileManagerActor.Tell(msg);
            });

            Receive<FileAddedMessage>(msg =>
            {
                _uiUpdater.UpdateFileList(msg.FilePath, msg.Metadata, msg.AlbumArt);
            });

            Receive<UpdateMetadataMessage>(msg =>
            {
                _metadataActor.Tell(msg);
            });


            Receive<FileAddedMessage>(msg => _uiUpdater.UpdateFileList(msg.FilePath, msg.Metadata, msg.AlbumArt));
            Receive<ConversionProgressMessage>(msg => _uiUpdater.UpdateProgress(msg.FilePath, msg.Progress));
            Receive<ConversionCompleteMessage>(HandleConversionComplete);
            Receive<UpdateStatusMessage>(msg => _uiUpdater.UpdateStatus(msg.Message));
            Receive<SelectFileInListViewMessage>(msg => _uiUpdater.SelectFileInListView(msg.FilePath));

            // Handle conversion workflow
            ReceiveAsync<ConvertFilesMessage>(HandleConvertFiles);
        }

        private async Task HandleConvertFiles(ConvertFilesMessage msg)
        {
            try
            {
                if (!_supportedFormats.Contains(msg.OutputFormat))
                {
                    _uiUpdater.UpdateStatus($"Unsupported format: {msg.OutputFormat}");
                    return;
                }

                var fileList = await _fileManagerActor
                    .Ask<List<string>>(new GetSelectedFilesMessage(), TimeSpan.FromSeconds(5));

                if (fileList == null || fileList.Count == 0)
                {
                    _uiUpdater.UpdateStatus("No files selected for conversion.");
                    return;
                }

                _filesToConvert.Clear();
                _retryCounts.Clear();

                foreach (var file in fileList)
                {
                    _filesToConvert.Enqueue(file);
                    _retryCounts[file] = 0;
                }

                _currentOutputFormat = msg.OutputFormat;
                _uiUpdater.EnableControls(false);
                _uiUpdater.UpdateStatus($"Starting conversion of {fileList.Count} file(s) to {msg.OutputFormat.ToUpper()}...");

                ConvertNextFile();
            }
            catch (Exception ex)
            {
                var errMsg = $"Error starting conversion: {ex.Message}";
                _uiUpdater.UpdateStatus(errMsg);
                _log.Error(ex, errMsg);
                _uiUpdater.EnableControls(true);
            }
        }

        private void HandleConversionComplete(ConversionCompleteMessage msg)
        {
            _uiUpdater.UpdateProgress(msg.FilePath, 100);

            if (msg.Success)
            {
                _uiUpdater.UpdateStatus($"Completed: {Path.GetFileName(msg.FilePath)}");
            }
            else
            {
                _log.Warning("Conversion failed for {0}: {1}", msg.FilePath, msg.ErrorMessage);
                _uiUpdater.UpdateStatus($"Error: {Path.GetFileName(msg.FilePath)} — {msg.ErrorMessage}");

                // Retry logic
                if (_retryCounts.TryGetValue(msg.FilePath, out var attempts) && attempts < MaxRetriesPerFile)
                {
                    _retryCounts[msg.FilePath]++;
                    _uiUpdater.UpdateStatus($"Retrying {Path.GetFileName(msg.FilePath)} (attempt {_retryCounts[msg.FilePath]} of {MaxRetriesPerFile})...");
                    _filesToConvert.Enqueue(msg.FilePath);
                }
            }

            ConvertNextFile();
        }

        private void ConvertNextFile()
        {
            if (_filesToConvert.Count > 0)
            {
                var file = _filesToConvert.Dequeue();
                _conversionActor.Tell(new ConvertFileMessage(file, _currentOutputFormat));
            }
            else
            {
                _uiUpdater.UpdateStatus("Conversion process finished.");
                _uiUpdater.EnableControls(true);
            }
        }

        protected override SupervisorStrategy SupervisorStrategy()
        {
            return new OneForOneStrategy(
                maxNrOfRetries: 3,
                withinTimeRange: TimeSpan.FromSeconds(30),
                localOnlyDecider: ex =>
                {
                    var msg = $"Actor error: {ex.Message}";
                    _uiUpdater.UpdateStatus(msg);
                    _log.Error(ex, msg);
                    return Directive.Restart;
                });
        }
    }
}