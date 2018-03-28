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

namespace ImageService.Controller.Handlers
{
    public class DirectoyHandler : IDirectoryHandler
    {
        #region Members
        private IImageController m_controller;              // The Image Processing Controller
        private ILoggingService m_logging;
        private FileSystemWatcher m_dirWatcher;             // The Watcher of the Dir
        private string m_path;                              // The Path of directory
        #endregion

        // The Event That Notifies that the Directory is being closed
        public event EventHandler<DirectoryCloseEventArgs> DirectoryClose;

        public DirectoyHandler(String path, IImageController controller)
        {
            this.m_path = path;
            this.m_controller = controller;
        }

        // The Function Recieves the directory to Handle
        public void StartHandleDirectory(string dirPath)
        {
            m_dirWatcher.Path = dirPath;
            m_dirWatcher.Filter = "*.jpg;*.gif;*.bmp;*.png";
            m_dirWatcher.Created += new FileSystemEventHandler(OnCreated);
            m_dirWatcher.EnableRaisingEvents = true;
        }

        private void OnCreated(object sender, FileSystemEventArgs e)
        {
            bool result;
            m_controller.ExecuteCommand(1, new String[] { e.FullPath, e.Name }, out result);
        }

        // The Event that will be activated upon new Command
        public void OnCommandRecieved(object sender, CommandRecievedEventArgs e)
        {
            if (this.m_path.Equals(e.RequestDirPath))
            { 
            }
        }

        public void onClose()
        {
            DirectoryClose(this, new DirectoryCloseEventArgs(m_path, "closing directory"));
            m_dirWatcher.EnableRaisingEvents = false;
            m_dirWatcher.Dispose();
        }
    }
}
