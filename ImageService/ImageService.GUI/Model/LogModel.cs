﻿using ImageService.Communication;
using ImageService.Communication.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.GUI.Model
{
    class LogModel : INotifyPropertyChanged
    {
        CommandMessage log;
        TcpClientChannel client { get; set; }

        public LogModel()
        {
            //connecting for the first time to the server and send "log command" command.
            this.client = new TcpClientChannel();
            client.SendCommand(new ImageService.Communication.Model.CommandMessage(3, "LogCommand"));

            this.log = client.RecieveCommand();
        }

        #region Notify Changed
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
        #endregion

        private ObservableCollection<string> mes;
        public ObservableCollection<string> Log
        {
            get { return mes; }
            set
            {
                mes = value;
                OnPropertyChanged("Log");
            }
        }
    }
}
