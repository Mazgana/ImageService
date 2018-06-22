using ImageService.Controller;
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
using ImageService.Logging.Modal;
using ImageService.Communication.Model;

namespace ImageService.Server
{
    public class ImageServer
    {
        #region Members
        private IImageController m_controller;
        private ILoggingService m_logging;
        TcpServerChannel tcpServer;
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
            string connected = "";

            for (int i = 0; i < handlersDirectories.Length; i++)
            {
                current = CreateHandler(handlersDirectories[i]);
                if (current != null)
                {
                    connected = connected + ";" + handlersDirectories[i];
                }
            }

            //creating new value in app config that holds only the handlers that thier creation succeeded
            ConfigurationManager.AppSettings["Handler"] = connected;

            ClientHandler ch = new ClientHandler();
            m_logging.Log("starting server", Logging.Modal.MessageTypeEnum.INFO);
            this.tcpServer = new TcpServerChannel(8000, ch, m_logging);
            ch.CommandRecieved += GetCommand;

            string[] firstHandler = connected.Split(';');
            for(int i=0;i<firstHandler.Length;i++)
            {
                if (firstHandler[i].StartsWith("C"))
                {
                    this.tcpServer.Start(firstHandler[i].ToString());
                    break;
                }
            }
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
                m_logging.Log("starting handler for directory: " + directory, MessageTypeEnum.INFO);

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
            m_logging.Log("sending command from server.", MessageTypeEnum.INFO);
            CommandRecieved?.Invoke(this, e);
        }
        
        /// <summary>
        /// getting a command from client to be execitind and replying
        /// </summary>
        /// <param name="sender"> of the command </param>
        /// <param name="e"> command details to be executed</param>
        public void GetCommand(object sender, CommandRecievedEventArgs e)
        {
            string curr = "";
            switch(e.CommandID)
            {
                case 1:
                    curr = "'add new file'";
                    break;
                case 2:
                    curr = "'get config'";
                    break;
                case 3:
                    curr = "'get log'";
                    break;
                case 4:
                    curr = "'close handler'";
                    break;
            }
            m_logging.Log("Getting command " + curr + " from client and execute it.", MessageTypeEnum.INFO);
            bool resultSuccess;

            //executing command and saving execution result
            string res = m_controller.ExecuteCommand(e.CommandID, e.Args, out resultSuccess);
            if (!resultSuccess)
            {
                m_logging.Log("execition failed. error: " + res, MessageTypeEnum.ERROR);
            }
            m_logging.Log("command executed", MessageTypeEnum.INFO);

            //if handlers were erased from config, close handler classes
            if(e.CommandID == 4){
                m_logging.Log("closing directory: " + e.RequestDirPath, MessageTypeEnum.INFO);
                CloseDirHandler(e.RequestDirPath);
            }

            //return the result to client
            CommandMessage response = new CommandMessage(e.CommandID, res);
            this.tcpServer.notifyAll(response);
            m_logging.Log("notified all", MessageTypeEnum.INFO);
        }
        
        /// <summary>
        /// updating all clients that a new message was updated in log
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"> message details to be updated</param>
        public void UpdateLog(object sender, MessageRecievedEventArgs e)
        {
            string mes = e.Status.ToString() + "|" + e.Message;
            CommandMessage response = new CommandMessage(5, mes);

            tcpServer.notifyAll(response);
        }

        /// <summary>
        /// Closing the handlers when sever is closing.
        /// </summary>
        public void CloseServer()
        {
            this.tcpServer.Stop();
            m_logging.Log("closing the server.", MessageTypeEnum.INFO);
            CloseCommand?.Invoke(this, null);
        }

        /// <summary>
        /// invoking all events that have to do with closing directory handler
        /// </summary>
        /// <param name="path"></param>
        public void CloseDirHandler(string path) {
            CloseCommand?.Invoke(this, new DirectoryCloseEventArgs(path, "close handler"));
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

