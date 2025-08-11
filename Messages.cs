using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuneTweak
{
    // Messages for Akka.NET actors
    public class SelectFilesMessage
    {
        public IEnumerable<string> Files { get; }
        public SelectFilesMessage(IEnumerable<string> files) => Files = files;
    }

    public class SelectFolderMessage
    {
        public string FolderPath { get; }
        public bool Recursive { get; }
        public SelectFolderMessage(string folderPath, bool recursive) => (FolderPath, Recursive) = (folderPath, recursive);
    }

    public class UpdateMetadataMessage
    {
        public string FilePath { get; }
        public string Title { get; }
        public string Artist { get; }
        public UpdateMetadataMessage(string filePath, string title, string artist) => (FilePath, Title, Artist) = (filePath, title, artist);
    }

    public class PlayFileMessage
    {
        public string FilePath { get; }
        public PlayFileMessage(string filePath) => FilePath = filePath;
    }

    public class ConvertFilesMessage
    {
        public string OutputFormat { get; }
        public ConvertFilesMessage(string outputFormat) => OutputFormat = outputFormat;
    }

    public class FileAddedMessage
    {
        public string FilePath { get; }
        public Dictionary<string, string> Metadata { get; }
        public Image AlbumArt { get; }
        public FileAddedMessage(string filePath, Dictionary<string, string> metadata, Image albumArt)
        {
            FilePath = filePath;
            Metadata = metadata;
            AlbumArt = albumArt;
        }
    }

    public class ConversionProgressMessage
    {
        public string FilePath { get; }
        public int Progress { get; }
        public ConversionProgressMessage(string filePath, int progress) => (FilePath, Progress) = (filePath, progress);
    }

    public class ConversionCompleteMessage
    {
        public string FilePath { get; }
        public bool Success { get; }
        public string ErrorMessage { get; }
        public ConversionCompleteMessage(string filePath, bool success, string errorMessage = "") => (FilePath, Success, ErrorMessage) = (filePath, success, errorMessage);
    }

    public class ReadMetadataMessage
    {
        public string FilePath { get; }
        public ReadMetadataMessage(string filePath) => FilePath = filePath;
    }

    public class GetSelectedFilesMessage { }

    public class SelectFileInListViewMessage
    {
        public string FilePath { get; }
        public SelectFileInListViewMessage(string filePath) => FilePath = filePath;
    }

    public class StopPlaybackMessage { }

    public class UpdateStatusMessage
    {
        public string Message { get; }
        public UpdateStatusMessage(string message) => Message = message;
    }

    public class ConvertFileMessage
    {
        public string FilePath { get; }
        public string OutputFormat { get; }
        public ConvertFileMessage(string filePath, string outputFormat) => (FilePath, OutputFormat) = (filePath, outputFormat);
    }


}
