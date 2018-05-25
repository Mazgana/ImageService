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
    class SettingsModel : INotifyPropertyChanged
    {
        String config;
        TcpClientChannel client { get; set; }

        public SettingsModel()
        {
            //connecting for the first time to the server and send "get config" command.

            //   System.Threading.Thread.Sleep(500);
            //   Console.WriteLine("after sleeping..");

            /*   this.config = client.RecieveCommand();

                //spliting config to members
                string[] configSrtings = config.MessageResponse.Split('|');

                OutputDirectory = configSrtings[0];
                SourceName = configSrtings[1];
                log_name = configSrtings[2];
                ThumbnailSize = configSrtings[3];

                this.handlers = new ObservableCollection<string>();

                string[] handlersDirectories = configSrtings[4].Split(';');
                for (int i = 0; i < handlersDirectories.Length; i++)
                {
                    if(handlersDirectories[i].Length != 0)
                        this.handlers.Add(handlersDirectories[i]);
                }
            */
            handlers = new ObservableCollection<string>();
            Object locker = new Object();
            BindingOperations.EnableCollectionSynchronization(Handlers, locker);
            this.client = TcpClientChannel.getInstance();

            if (this.client.IsConnected)
            {
                client.UpdateModel += ViewUpdate;
                client.SendCommand(new ImageService.Communication.Model.CommandMessage(2, null));
            }
            /*
            OutputDirectory = "loading..";
            SourceName = "loading..";
            log_name = "loading name..";
            ThumbnailSize = "loading size..";
            */
   //         this.handlers = new ObservableCollection<string>();
            /*
            string[] handlersDirectories = { "directory1", "directory2", "directory3" };
            for (int i = 0; i < handlersDirectories.Length; i++)
            {
                if (handlersDirectories[i].Length != 0)
                    this.handlers.Add(handlersDirectories[i]);
            }*/

     //       Thread.Sleep(1000);
        }

        private void ViewUpdate(object sender, CommandRecievedEventArgs e)
        {
            if(e.CommandID == 2)
            {
                this.config = e.Args[0];
                string[] configSrtings = config.Split('|');

                OutputDirectory = configSrtings[0];
                SourceName = configSrtings[1];
                log_name = configSrtings[2];
                ThumbnailSize = configSrtings[3];

     //           this.handlers = new ObservableCollection<string>();

                string[] handlersDirectories = configSrtings[4].Split(';');
                for (int i = 0; i < handlersDirectories.Length; i++)
                {
                    if (handlersDirectories[i].Length != 0)
                        this.handlers.Add(handlersDirectories[i]);
                }
            }
            if(e.CommandID == 4)
            {
                string res = e.Args[0];
                if (res.Equals("closed"))
                {
                    this.handlers.Remove(e.Args[1]);
                }
            }
        }

        #region Notify Changed
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            Console.WriteLine("--hey some property changed:)");
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion

        private string output_dir;
        public string OutputDirectory
        {
            get { return output_dir; }
            set
            {
                Console.WriteLine("writing output directory");
                output_dir = value;
                OnPropertyChanged("Output Directory");
            }
        }

        private string src_name;
        public string SourceName
        {
            get { return src_name; }
            set
            {
                src_name = value;
                OnPropertyChanged("Source Name");
            }
        }

        private string log_name;
        public string LogName
        {
            get { return log_name; }
            set
            {
                log_name = value;
                OnPropertyChanged("Log Name");
            }
        }

        private string thumb_size;
        public string ThumbnailSize
        {
            get { return thumb_size; }
            set
            {
                thumb_size = value;
                OnPropertyChanged("Thumbnail Size");
            }
        }

        private ObservableCollection<string> handlers;
        public ObservableCollection<string> Handlers
        {
            get { return handlers; }
            set
            {
                handlers = value;
                OnPropertyChanged("Handlers");
            }
        }

        private string selected;
        public string SelectedHandler
        {
            get { return selected; }
            set
            {
                selected = value;
                OnPropertyChanged("Selected Handler");
            }
        }

        private bool connect;
        public bool IsConnected
        {
            get { return this.client.IsConnected; }
            set
            {
                connect = this.client.IsConnected;
                OnPropertyChanged("Is server connected");
            }
        }

        public bool removeHandler(string handler)
        {
            this.client.SendCommand(new ImageService.Communication.Model.CommandMessage(4, handler));

        /*    string res = client.RecieveCommand().MessageResponse;
            if (res.Equals("closed"))
            {
                this.handlers.Remove(handler);
                return true;
            }
        */
            return false;
        }
    }
}
