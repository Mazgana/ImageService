using ImageService.Communication;
using ImageService.Communication.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Newtonsoft.Json;
using System.Windows.Data;

namespace ImageService.GUI.Model
{
    class LogModel : INotifyPropertyChanged
    {
        bool isRunning;
        bool gotLog;
        String log;
        TcpClientChannel client { get; set; }

        public LogModel()
        {
            isRunning = false;
            //connecting for the first time to the server and send "log command" command.
            this.client = TcpClientChannel.getInstance();
            this.LogMes = new ObservableCollection<MsgRecievedEventArgs>();
            Object locker = new Object();
            BindingOperations.EnableCollectionSynchronization(LogMes, locker);

            if (this.client.IsConnected)
            {
                isRunning = true;
                client.UpdateModel += ViewLogUpdate;
                client.SendCommand(new ImageService.Communication.Model.CommandMessage(3, null));
            }
        }

        private void ViewLogUpdate(object sender, CommandRecievedEventArgs e)
        {
            if(e.CommandID == 3 && LogMes.Count < 2)
            {
                this.log = e.Args[0];
                List<string> allLog = JsonConvert.DeserializeObject<List<String>>(this.log);
                string[] current;

                foreach (String st in allLog)
                {
                    current = st.Split('|');
                    this.LogMes.Add(new MsgRecievedEventArgs(current[0], current[1]));
                }
                gotLog = true;
            }
            if (e.CommandID == 5 && isRunning && gotLog)
            {
                string[] mes;
                string entry = e.Args[0];
                mes = e.Args[0].Split('|');
                this.LogMes.Add(new MsgRecievedEventArgs(mes[0], mes[1]));
            }
        }

        #region Notify Changed
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
        #endregion

        private ObservableCollection<MsgRecievedEventArgs> mes;
        public ObservableCollection<MsgRecievedEventArgs> LogMes
        {
            get { return mes; }
            set
            {
                mes = value;
                OnPropertyChanged("LogMes");
            }
        }
    }
}
