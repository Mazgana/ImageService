using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.GUI.Model
{
    class SettingsModel : INotifyPropertyChanged
    {
        #region Notify Changed
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
        #endregion

        private string output_dir;
        public string OutputDirectory
        {
            get { return output_dir; }
            set
            {
                output_dir = value;
                OnPropertyChanged("Output Directory");
            }
        }

        private string src_name;
        public string SourceName
        {
            get { return src_name; }
            set
            {
                src_name = value;
                OnPropertyChanged("Source Name");
            }
        }

        private string log_name;
        public string LogName
        {
            get { return log_name; }
            set
            {
                log_name = value;
                OnPropertyChanged("Log Name");
            }
        }

        private string thumb_size;
        public string ThumbnailSize
        {
            get { return thumb_size; }
            set
            {
                thumb_size = value;
                OnPropertyChanged("Thumbnail Size");
            }
        }
    }
}
