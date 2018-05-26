using ImageService.GUI.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.GUI.ViewModels
{
    class MainWindowViewModel : INotifyPropertyChanged
    {
        #region Notify Changed
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
        protected void NotifyPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private MainWindowModel mainWindowModel;

        public MainWindowViewModel()
        {
            this.mainWindowModel = new MainWindowModel();
        }

        public bool IsConnected
        {
            get { return this.mainWindowModel.IsConnected; }
        }
    }
}
