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
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
        private SettingsModel settingsModel;

        public SettingsViewModel(SettingsModel model)
        {
            this.settingsModel = model;

            SettingsModel.PropertyChanged +=
       delegate (Object sender, PropertyChangedEventArgs e) {
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
    }
}
