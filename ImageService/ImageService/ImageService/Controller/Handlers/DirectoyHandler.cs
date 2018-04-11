﻿using ImageService.Modal;
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

namespace ImageService.Controller.Handlers
{
    public class DirectoyHandler : IDirectoryHandler
    {
        #region Members
        private IImageController m_controller;              // The Image Processing Controller
        private ILoggingService m_logging;
        private FileSystemWatcher m_dirWatcher;             // The Watcher of the Dir
        private string m_path;                              // The Path of directory
        DateTime lastRead = DateTime.MinValue;
        #endregion

        // The Event That Notifies that the Directory is being closed
        public event EventHandler<DirectoryCloseEventArgs> DirectoryClose;

        public DirectoyHandler(String path, IImageController controller, ILoggingService logging)
        {
            this.m_path = path;
            this.m_controller = controller;
            this.m_logging = logging;
            m_dirWatcher = new FileSystemWatcher();
        }

        // The Function Recieves the directory to Handle
        public bool StartHandleDirectory(string dirPath)
        {
            if (System.IO.Directory.Exists(dirPath))
            {
                m_logging.Log("handling directory: " + dirPath, MessageTypeEnum.INFO);
                m_dirWatcher.Path = dirPath;
                DirectoryClose += onClose;
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
        
        private void OnChanged(object sender, FileSystemEventArgs e)
        {         
                DateTime lastWriteTime = File.GetLastWriteTime(e.FullPath);
                if (lastWriteTime != this.lastRead && isImage(e.FullPath))
                {
                    this.lastRead = lastWriteTime;
                    m_logging.Log("directory changed: " + e.Name, MessageTypeEnum.INFO);
                    m_logging.Log("change type: " + e.ChangeType.GetType(), MessageTypeEnum.INFO);
                    OnCommandRecieved(this, new CommandRecievedEventArgs(1, new String[] { e.FullPath, e.Name }, e.FullPath));
                }
        }

        private bool isImage(string path)
        {
            string ext = new FileInfo(path).Extension;
            if (ext.Equals(".jpg") || ext.Equals(".png") || ext.Equals(".bmp") || ext.Equals(".gif"))
                return true;

            return false;
        }

        // The Event that will be activated upon new Command
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

        public void onClose(object send, DirectoryCloseEventArgs e)
        {
            DirectoryClose -= onClose;
            m_logging.Log("closing handler for directory: " + m_path, MessageTypeEnum.INFO);
            DirectoryClose(this, e);
            m_dirWatcher.EnableRaisingEvents = false;
            m_dirWatcher.Dispose();
        }
    }
}
