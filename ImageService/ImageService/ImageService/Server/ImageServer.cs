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
using System.Configuration;

namespace ImageService.Server
{
    public class ImageServer
    {
        #region Members
        private IImageController m_controller;
        private ILoggingService m_logging;
        private List<DirectoyHandler> handlersList;
        #endregion

        #region Properties
        // The event that notifies about a new Command being recieved
        public event EventHandler<CommandRecievedEventArgs> CommandRecieved;          
        #endregion

        public ImageServer(ILoggingService logging, IImageServiceModal modal)
        {
            this.m_logging = logging;
            this.m_controller = new ImageController(modal);

            handlersList = new List<DirectoyHandler>();
            DirectoyHandler current;

            string directories = ConfigurationManager.AppSettings["Handler"];
            string[] handlersDirectories = directories.Split(';');
            for (int i = 0; i < handlersDirectories.Length; i++)
            {
                current = CreateHandler(handlersDirectories[i]);
                handlersList.Add(current);
            }
        }

        public DirectoyHandler CreateHandler(String directory)
        {
            DirectoyHandler h = new DirectoyHandler(directory, m_controller, m_logging);
            CommandRecieved += h.OnCommandRecieved;
            h.DirectoryClose += OnCloseServer;
            h.StartHandleDirectory(directory);
            m_logging.Log("starting handler for directory: " + directory, Logging.Modal.MessageTypeEnum.INFO);

            return h;
        }
        
        public void SendCommand()
        {

        }

        public void OnCloseServer(object sender, DirectoryCloseEventArgs e)
        {
            DirectoyHandler dh = (DirectoyHandler)sender;
            CommandRecieved -= dh.OnCommandRecieved;
            dh.DirectoryClose -= OnCloseServer;
        }
    }
}

