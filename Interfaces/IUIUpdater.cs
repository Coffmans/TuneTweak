using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuneTweak.Interfaces
{
    public interface IUIUpdater
    {
        void UpdateFileList(string filePath, Dictionary<string, string> metadata, Image albumArt);
        void UpdateMetadata(string filePath, string title, string artist);
        void UpdateProgress(string filePath, int progress);
        void UpdateStatus(string message);
        void EnableControls(bool enable);
        void SelectFileInListView(string filePath);
    }
}
