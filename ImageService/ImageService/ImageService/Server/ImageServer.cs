using ImageService.Controller;
using ImageService.Controller.Handlers;
using ImageService.Infrastructure.Enums;
using ImageService.Logging;
using ImageService.Modal;
using ImageService.Modal.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Server
{
    public class ImageServer
    {
        #region Members
        private IImageController m_controller;
        private ILoggingService m_logging;
        #endregion

        #region Properties
        public event EventHandler<CommandRecievedEventArgs> CommandRecieved;          // The event that notifies about a new Command being recieved
        public event EventHandler<DirectoryCloseEventArgs> CloseHandler;
        #endregion

        public void CreateHandler(String directory)
        {
            DirectoyHandler h = new DirectoyHandler(directory, m_controller);
            CommandRecieved += h.OnCommandRecieved;
            h.DirectoryClose += CloseHandler;
        }
        
        public void SendCommand()
        {

        }

        public void OnCloseServer(object sender)
        {

        }
    }
}
