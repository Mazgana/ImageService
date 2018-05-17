using ImageService.Communication;
using ImageService.Communication.Model;
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
        CommandMessage config;

        public SettingsModel()
        {
            //connecting for the first time to the server and send "get config" command.
            TcpClientChannel client = new TcpClientChannel();
            client.SendCommand(new ImageService.Communication.Model.CommandMessage(2, "GetConfig"));
            this.config = client.RecieveCommand();
            client.Stop();

            //spliting config to members
            string[] configSrtings = config.MessageResponse.Split('|');

            //the last part of the config tring is the handlers
            OutputDirectory = configSrtings[0];
            SourceName = configSrtings[1];
            log_name = configSrtings[2];
            ThumbnailSize = configSrtings[3];
        }

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
