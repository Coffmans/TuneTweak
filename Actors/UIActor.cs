using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuneTweak.Interfaces;

namespace TuneTweak.Actors
{
    public class UIActor : ReceiveActor
    {
        //private readonly IUIUpdater _uiUpdater;

        //public UIActor(IUIUpdater uiUpdater)
        //{
        //    _uiUpdater = uiUpdater;

        //    Receive<EnableControlsMessage>(msg => _uiUpdater.EnableControls(msg.Enable));
        //    Receive<UpdateFileListMessage>(msg => _uiUpdater.UpdateFileList(msg.FilePath, msg.Metadata, msg.AlbumArt));
        //    Receive<UpdateProgressMessage>(msg => _uiUpdater.UpdateProgress(msg.FilePath, msg.Progress));
        //    Receive<UpdateStatusMessage>(msg => _uiUpdater.UpdateStatus(msg.Message));
        //    // Add more mappings as needed...
        //}
    }
}
