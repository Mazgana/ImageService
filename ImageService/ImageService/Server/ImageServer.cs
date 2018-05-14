﻿using ImageService.Controller;
using ImageService.Controller.Handlers;
using ImageService.Infrastructure.Enums;
using ImageService.Communication;
using ImageService.Logging;
using ImageService.Modal;
using ImageService.Modal.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using ImageService.Communication.Interfaces;

namespace ImageService.Server
{
    public class ImageServer
    {
        #region Members
        private IImageController m_controller;
        private ILoggingService m_logging;
        TcpServerChannel tcpServer;
       // private List<DirectoyHandler> handlersList;
        #endregion

        #region Properties
        public event EventHandler<CommandRecievedEventArgs> CommandRecieved;         // The event that notifies about a new Command being recieved
        public event EventHandler<DirectoryCloseEventArgs> CloseCommand;            // The event that notifies that the service is close and that the server should close
        #endregion

        /// <summary>
        /// Image server constructor, create handler for each path in the app config file.
        /// </summary>
        /// <param name="logging"> The application's logger </param>
        /// <param name="modal"> The service modal </param>
        /// <param name="controller"> The image controller </param>
        public ImageServer(ILoggingService logging, IImageServiceModal modal, IImageController controller)
        {
            this.m_logging = logging;
            this.m_controller = controller;

            //Extract all directories from app config file.
            DirectoyHandler current;
            string directories = ConfigurationManager.AppSettings["Handler"];
            string[] handlersDirectories = directories.Split(';');

            for (int i = 0; i < handlersDirectories.Length; i++)
            {
                current = CreateHandler(handlersDirectories[i]);
            }
            IClientHandler ch = new ClientHandler();
            m_logging.Log("starting server", Logging.Modal.MessageTypeEnum.INFO);
            this.tcpServer = new TcpServerChannel(8000,ch, m_logging);
            this.tcpServer.Start();
        }

        /// <summary>
        /// Create new handler for the given path.
        /// </summary>
        /// <param name="directory"> The folder that the new handler will listen to. </param>
        /// <returns> The new handler. If the handler's creation failed - return null. </returns>
        public DirectoyHandler CreateHandler(String directory)
        {
            DirectoyHandler h = new DirectoyHandler(directory, m_controller, m_logging);

            if (h.StartHandleDirectory(directory))
            {
                CommandRecieved += h.OnCommandRecieved;
                h.DirectoryClose += OnCloseServer;
                CloseCommand += h.OnClose;
                m_logging.Log("starting handler for directory: " + directory, Logging.Modal.MessageTypeEnum.INFO);

                return h;
            } else
            {
                return null;
            }
        }
        
        /// <summary>
        /// sending the given command from the server to all the application's handlers.
        /// </summary>
        /// <param name="e"> The command that will be sent. </param>
        public void SendCommand(CommandRecievedEventArgs e)
        {
            m_logging.Log("sending command from server.", Logging.Modal.MessageTypeEnum.INFO);
            CommandRecieved?.Invoke(this, e);
        }

        /// <summary>
        /// Closing the handlers when sever is closing.
        /// </summary>
        public void CloseServer()
        {
            this.tcpServer.Stop();
            m_logging.Log("closing the server.", Logging.Modal.MessageTypeEnum.INFO);
            CloseCommand?.Invoke(this, null);
        }

        /// <summary>
        /// Closing the server - handler will call this function to tell server it closed.
        /// </summary>
        /// <param name="sender"> Handler as sender. </param>
        /// <param name="e"> Information on the handler - dir and a message. </param>
        public void OnCloseServer(object sender, DirectoryCloseEventArgs e)
        {
            DirectoyHandler dh = (DirectoyHandler)sender;
            CommandRecieved -= dh.OnCommandRecieved;
            dh.DirectoryClose -= OnCloseServer;
            CloseCommand -= dh.OnClose;
        }
    }
}

