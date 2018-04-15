﻿using ImageService.Controller;
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
    /*
     * Server class. recieves and sends commands. creates and holds all handlers. 
     */
    public class ImageServer
    {
        #region Members
        private IImageController m_controller;
        private ILoggingService m_logging;
        private List<DirectoyHandler> handlersList;
        #endregion

        #region Properties
        public event EventHandler<CommandRecievedEventArgs> CommandRecieved;         // The event that notifies about a new Command being recieved
        public event EventHandler<DirectoryCloseEventArgs> CloseCommand;            // The event that notifies that the service is close and that the server should close
        #endregion

        // constructor.
        public ImageServer(ILoggingService logging, IImageServiceModal modal, IImageController controller)
        {
            this.m_logging = logging;
            this.m_controller = controller;

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

            if (h.StartHandleDirectory(directory))
            {
                CommandRecieved += h.OnCommandRecieved;
                h.DirectoryClose += OnCloseServer;
                CloseCommand += h.onClose;
                m_logging.Log("starting handler for directory: " + directory, Logging.Modal.MessageTypeEnum.INFO);

                return h;
            } else
            {
                return null;
            }
        }
        
        public void SendCommand(CommandRecievedEventArgs e)
        {
            CommandRecieved?.Invoke(this, e);
        }

        public void CloseServer()
        {
            m_logging.Log("closing the server.", Logging.Modal.MessageTypeEnum.INFO);
            CloseCommand?.Invoke(this, null);
        }

        public void OnCloseServer(object sender, DirectoryCloseEventArgs e)
        {
            m_logging.Log("removing events. final closing", Logging.Modal.MessageTypeEnum.INFO);
            DirectoyHandler dh = (DirectoyHandler)sender;
            CommandRecieved -= dh.OnCommandRecieved;
            dh.DirectoryClose -= OnCloseServer;
            CloseCommand -= dh.onClose;
        }
    }
}

