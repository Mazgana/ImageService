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

        /// <summary>
        /// notifying and updating properties that are binded.
        /// </summary>
        /// <param name="name"></param>
        protected void NotifyPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        private SettingsModel settingsModel;

        /// <summary>
        /// constructor for VM
        /// </summary>
        public SettingsViewModel()
        {
            this.settingsModel = new SettingsModel();
            SettingsModel.PropertyChanged += delegate (Object sender, PropertyChangedEventArgs e) {
                NotifyPropertyChanged(e.PropertyName);
            };
       }

        /// <summary>
        /// setting property.
        /// </summary>
        public SettingsModel SettingsModel
        {
            get { return this.settingsModel; }
            set
            {
                this.settingsModel = value;
            }
        }

        /// <summary>
        /// removing handler from handlers list property
        /// </summary>
        /// <param name="handler"> to remove from list</param>
        /// <returns>true if removed, flase if not found or not removed</returns>
        public bool removeHandler(string handler)
        {
            return this.settingsModel.removeHandler(handler);
        }

        /// <summary>
        /// output directory property.
        /// </summary>
        public string OutputDirectory
        {
            get { return this.settingsModel.OutputDirectory; }
        }

        /// <summary>
        /// source name property.
        /// </summary>
        public string SourceName
        {
            get { return this.settingsModel.SourceName; }
        }

        /// <summary>
        /// log name property.
        /// </summary>
        public string LogName
        {
            get { return this.settingsModel.LogName; }
        }

        /// <summary>
        /// thumbnail size property.
        /// </summary>
        public string ThumbnailSize
        {
            get { return this.settingsModel.ThumbnailSize; }
        }
    }
}
