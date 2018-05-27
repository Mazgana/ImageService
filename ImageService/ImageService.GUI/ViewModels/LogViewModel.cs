using ImageService.Communication.Model;
using ImageService.GUI.Model;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace ImageService.GUI.ViewModels
{
    class LogViewModel : INotifyPropertyChanged
    {
        #region Notify Changed
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
        protected void NotifyPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private LogModel logModel;

        /// <summary>
        /// Log view model constructor.
        /// </summary>
        public LogViewModel()
        {
            this.logModel = new LogModel();

            LogModel.PropertyChanged +=
       delegate (Object sender, PropertyChangedEventArgs e) {
           NotifyPropertyChanged(e.PropertyName);
       };
        }

        public LogModel LogModel
        {
            get { return this.logModel; }
            set
            {
                this.logModel = value;
            }
        }

        /// <summary>
        /// Log messages data structure property.
        /// </summary>
        public ObservableCollection<MsgRecievedEventArgs> LogMes
        {
            get { return this.logModel.LogMes; }
        }
    }
}
