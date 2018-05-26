using ImageService.GUI.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.GUI.ViewModels
{
    class SettingsViewModel : INotifyPropertyChanged
    {
        #region Notify Changed
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
        protected void NotifyPropertyChanged(string name)
        {
            Console.WriteLine("invoking property changed in viem model");
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        private SettingsModel settingsModel;

        public SettingsViewModel()
        {
            this.settingsModel = new SettingsModel();
            Console.WriteLine("now adding delegate");
            SettingsModel.PropertyChanged += delegate (Object sender, PropertyChangedEventArgs e) {
                NotifyPropertyChanged(e.PropertyName);
            };
       }

        public SettingsModel SettingsModel
        {
            get { return this.settingsModel; }
            set
            {
                this.settingsModel = value;
            }
        }

        public bool removeHandler(string handler)
        {
            return this.settingsModel.removeHandler(handler);
        }

        public string OutputDirectory
        {
            get { return this.settingsModel.OutputDirectory; }
        }

        public string SourceName
        {
            get { return this.settingsModel.SourceName; }
        }

        public string LogName
        {
            get { return this.settingsModel.LogName; }
        }

        public string ThumbnailSize
        {
            get { return this.settingsModel.ThumbnailSize; }
        }

        //public bool IsConnected
        //{
        //    get { return this.settingsModel.IsConnected; }
        //}
    }
}
