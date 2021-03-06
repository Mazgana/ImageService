﻿using ImageService.Communication;
using ImageService.Communication.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Threading;
using System.Windows.Data;

namespace ImageService.GUI.Model
{
    /// <summary>
    /// The settings window model.
    /// </summary>
    class SettingsModel : INotifyPropertyChanged
    {
        TcpClientChannel client { get; set; }
        private string selected;
        private ObservableCollection<string> handlers { get; set; }
        private string thumb_size;
        private string log_name;
        private string src_name;
        private string output_dir;

        public SettingsModel()
        {
            //connecting for the first time to the server and send "get config" command.
            handlers = new ObservableCollection<string>();
            Object locker = new Object();
            BindingOperations.EnableCollectionSynchronization(handlers, locker);
            this.client = TcpClientChannel.getInstance();

            //If the client is connected to the server, continue with asking the config.
            if (this.client.IsConnected)
            {
                client.UpdateModel += ViewUpdate;
                client.SendCommand(new ImageService.Communication.Model.CommandMessage(2, null));
            }
        }

        /// <summary>
        /// Upates the window members to contain the app.config data as recieved from the server.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"> The recieved data from the server. </param>
        private void ViewUpdate(object sender, CommandRecievedEventArgs e)
        {
            //Checks if the server respose is the application config.
            if (e.CommandID == 2 && handlers.Count < 1)
            {
                //initialize the settings window's members to hold the configuration values.
                string config = e.Args[0];
                string[] configSrtings = config.Split('|');

                OutputDirectory = configSrtings[0];
                SourceName = configSrtings[1];
                LogName = configSrtings[2];
                ThumbnailSize = configSrtings[3];

                string[] handlersDirectories = configSrtings[4].Split(';');
                for (int i = 0; i < handlersDirectories.Length; i++)
                {
                    if (handlersDirectories[i].Length != 0)
                        this.handlers.Add(handlersDirectories[i]);
                }
            }

            //checks if the server's response is the close handler response. if it is - updates the handlers list.
            if(e.CommandID == 4)
            {
                string res = e.Args[0];
                if (handlers.Contains(res))
                {
                    handlers.Remove(res);
                }
            }
        }

        #region Notify Changed
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion

        /// <summary>
        /// All the config data in the settings members.
        /// </summary>
        public string OutputDirectory
        {
            get { return output_dir; }
            set
            {
                output_dir = value;
                OnPropertyChanged("OutputDirectory");
            }
        }

        public string SourceName
        {
            get { return src_name; }
            set
            {
                src_name = value;
                OnPropertyChanged("SourceName");
            }
        }

        public string LogName
        {
            get { return log_name; }
            set
            {
                log_name = value;
                OnPropertyChanged("LogName");
            }
        }

        public string ThumbnailSize
        {
            get { return thumb_size; }
            set
            {
                thumb_size = value;
                OnPropertyChanged("ThumbnailSize");
            }
        }

        public ObservableCollection<string> Handlers
        {
            get { return handlers; }
            set
            {
                handlers = value;
                OnPropertyChanged("Handlers");
            }
        }

        public string SelectedHandler
        {
            get { return selected; }
            set
            {
                selected = value;
                OnPropertyChanged("SelectedHandler");
            }
        }

        //Send the server 'close handler' command with the handle's name.
        public bool removeHandler(string handler)
        {
            this.client.SendCommand(new CommandMessage(4, handler));
            return true;
        }
    }
}
