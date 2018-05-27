using ImageService.Communication;
using ImageService.Communication.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Newtonsoft.Json;
using System.Windows.Data;
using System.Linq;

namespace ImageService.GUI.Model
{
    /// <summary>
    /// The log window model.
    /// </summary>
    class LogModel : INotifyPropertyChanged
    {
        bool isRunning;
        bool gotLog;
        String log;
        TcpClientChannel client { get; set; }
        private ObservableCollection<MsgRecievedEventArgs> mes;

        public LogModel()
        {
            isRunning = false;
            
            //connecting for the first time to the server and send "log command" command.
            this.client = TcpClientChannel.getInstance();
            this.LogMes = new ObservableCollection<MsgRecievedEventArgs>();
            Object locker = new Object();
            BindingOperations.EnableCollectionSynchronization(LogMes, locker);

            //If the client is connected to the server, continue with asking the log. 
            if (this.client.IsConnected)
            {
                isRunning = true;
                client.UpdateModel += ViewLogUpdate;
                client.SendCommand(new CommandMessage(3, null));
            }
        }

        /// <summary>
        /// Adding log events to the log message dats structure while the application is running
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"> The message that was recieved from the server. </param>
        private void ViewLogUpdate(object sender, CommandRecievedEventArgs e)
        {
            //Checks if the recieved data is relevant to the log view and if its the first update of the log data structure 
            if(e.CommandID == 3 && LogMes.Count < 2)
            {
                this.log = e.Args[0];
                List<string> allLog = JsonConvert.DeserializeObject<List<String>>(this.log);
                string[] current;

                foreach (String st in allLog)
                {
                    current = st.Split('|');
                    this.LogMes.Insert(0, new MsgRecievedEventArgs(current[0], current[1]));
                }
                gotLog = true;
            }

            //Checks if the recieved data is relevant to the log view and if its message from the current running
            // that was recieved in real time.
            if (e.CommandID == 5 && isRunning && gotLog)
            {
                string[] mes;
                string entry = e.Args[0];
                mes = e.Args[0].Split('|');
                this.LogMes.Insert(0, new MsgRecievedEventArgs(mes[0], mes[1]));
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
        
        /// <summary>
        /// The log messages data structure.
        /// </summary>
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
