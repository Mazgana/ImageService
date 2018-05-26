using ImageService.Communication;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.GUI.Model
{
    class MainWindowModel : INotifyPropertyChanged
    {
        TcpClientChannel client { get; set; }
        private bool connect;
        
        public MainWindowModel()
        {
            this.client = TcpClientChannel.getInstance();
        }

        #region Notify Changed
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion

        public bool IsConnected
        {
            get { return this.client.IsConnected; }
            set
            {
                connect = this.client.IsConnected;
                OnPropertyChanged("IsConnected");
            }
        }
    }
}
