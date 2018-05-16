using ImageService.Modal;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageService.Infrastructure;
using ImageService.Infrastructure.Enums;
using ImageService.Logging;
using ImageService.Logging.Modal;
using System.Text.RegularExpressions;
using ImageService.Modal.Event;
using ImageService.Communication.Model;

namespace ImageService.Controller.Handlers
{
    /*
     * this class handles a specific directory, watching it and making changes.              
     */ 
    public class DirectoyHandler : IDirectoryHandler
    {
        #region Members
        private IImageController m_controller;              // The Image Processing Controller
        private ILoggingService m_logging;                  // The image logger of the prgram
        private FileSystemWatcher m_dirWatcher;             // The Watcher of the Dir
        private string m_path;                              // The Path of directory
        DateTime lastRead = DateTime.MinValue;
        #endregion

        // The Event That Notifies that the Directory is being closed
        public event EventHandler<DirectoryCloseEventArgs> DirectoryClose;

        /// <summary>
        /// constructor. initializes directory handler.
        /// </summary>
        /// <param name="path"> path of specific directory that wil be handled by handler. </param>
        /// <param name="controller"> program's controller for executing commands. </param>
        /// <param name="logging"> program's logger for messaging. </param>
        public DirectoyHandler(String path, IImageController controller, ILoggingService logging)
        {
            this.m_path = path;
            this.m_controller = controller;
            this.m_logging = logging;
            m_dirWatcher = new FileSystemWatcher();
        }

        /// <summary>
        /// function that starts listening and handling the directory recieved.
        /// </summary>
        /// <param name="dirpath"> directory path to listen to for changes. </param>
        /// <returns> true if directory exists. false otherwise. </returns>
        public bool StartHandleDirectory(string dirPath)
        {
            if (System.IO.Directory.Exists(dirPath))
            {
                m_logging.Log("handling directory: " + dirPath, MessageTypeEnum.INFO);
                m_dirWatcher.Path = dirPath;
                m_dirWatcher.Changed += new FileSystemEventHandler(OnChanged);
                m_dirWatcher.EnableRaisingEvents = true;
                return true;    //the directory exists
            }
            else
            {
                m_logging.Log("could not find directory: " + dirPath, MessageTypeEnum.WARNING);
                return false;   //the directory isn't exists
            }
        }

        /// <summary>
        /// function will be called when a change occurs in directory, noticing logger and command event.
        /// </summary>
        /// <param name="sender"> the class from which the event was invoked to call this function. </param>
        /// <param name="e"> arguments for a file system event. </param>
        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            DateTime lastWriteTime = File.GetLastWriteTime(e.FullPath);
            m_logging.Log("directory changed: " + e.Name, MessageTypeEnum.INFO);
            m_logging.Log("change type: " + e.ChangeType.GetType(), MessageTypeEnum.INFO);
            OnCommandRecieved(this, new CommandRecievedEventArgs(1, new String[] { e.FullPath, e.Name }, e.FullPath));
        }

        /// <summary>
        /// checks if given file path is of type image.
        /// </summary>
        /// <param name="path"> file path to be checked. </param>
        /// <returns> true if file is an image. false otherwise. </returns>
        private bool isImage(string path)
        {
            string ext = new FileInfo(path).Extension;
            if (ext.Equals(".jpg") || ext.Equals(".png") || ext.Equals(".bmp") || ext.Equals(".gif"))
                return true;
            return false;
        }

        /// <summary>
        /// The Event that will be activated upon new Command
        /// </summary>
        /// <param name="sender"> the class from which the event was invoked to call this function. </param>
        /// <param name="e"> arguments for a command event. </param>
        public void OnCommandRecieved(object sender, CommandRecievedEventArgs e)
        {
            m_logging.Log("recieved command for directory: " + e.RequestDirPath, MessageTypeEnum.INFO);
            bool resultSuccess;
            string execResult = m_controller.ExecuteCommand(e.CommandID, e.Args, out resultSuccess);
            if (!resultSuccess)
            {
                m_logging.Log("execition failed. error: " + execResult, MessageTypeEnum.FAIL);
            }
        }

        /// <summary>
        /// The Event that will be activated when server closes
        /// </summary>
        /// <param name="sender"> the class (server) from which the event was invoked to call this function. </param>
        /// <param name="e"> arguments for a close event. </param>
        public void OnClose(object send, DirectoryCloseEventArgs e)
        {
            m_logging.Log("closing handler for directory: " + m_path, MessageTypeEnum.INFO);
            m_dirWatcher.EnableRaisingEvents = false;
            m_dirWatcher.Dispose();
            DirectoryClose?.Invoke(this, e);
        }
    }
}
