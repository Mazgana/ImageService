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
        /// <summary>
        /// raising events that have to do with properties that are changed.
        /// </summary>
        /// <param name="name"></param>
        protected void NotifyPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private MainWindowModel mainWindowModel;

        /// <summary>
        /// constructor.
        /// </summary>
        public MainWindowViewModel()
        {
            this.mainWindowModel = new MainWindowModel();
        }

        /// <summary>
        /// returns true if model is connected, false otherwise.
        /// </summary>
        public bool IsConnected
        {
            get { return this.mainWindowModel.IsConnected; }
        }
    }
}
