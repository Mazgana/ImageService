using ImageService.Communication.Model;
using ImageService.GUI.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public ObservableCollection<MsgRecievedEventArgs> LogMes
        {
            get { return this.logModel.LogMes; }
        }
    }
}
