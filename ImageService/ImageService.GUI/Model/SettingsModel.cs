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
        TcpClientChannel client { get; set; }

        public SettingsModel()
        {
            //connecting for the first time to the server and send "get config" command.
            this.client = new TcpClientChannel();
            client.SendCommand(new ImageService.Communication.Model.CommandMessage(2, "GetConfig"));
            this.config = client.RecieveCommand();
            client.Stop();

            //spliting config to members
            string[] configSrtings = config.MessageResponse.Split('|');

            OutputDirectory = configSrtings[0];
            SourceName = configSrtings[1];
            log_name = configSrtings[2];
            ThumbnailSize = configSrtings[3];

            this.handlers = new List<string>();

            string[] handlersDirectories = configSrtings[4].Split(';');
            for (int i = 0; i < handlersDirectories.Length; i++)
            {
                this.handlers.Add(handlersDirectories[i]);
            }

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

        private List<string> handlers;
        public List<string> Handlers
        {
            get { return handlers; }
            set
            {
                handlers = value;
                OnPropertyChanged("Handlers");
            }
        }

        private string selected;
        public string SelectedHandler
        {
            get { return selected; }
            set
            {
                selected = value;
                OnPropertyChanged("Selected Handler");
            }
        }

        public bool removeHandler(string handler)
        {
            this.client.SendCommand(new ImageService.Communication.Model.CommandMessage(4, handler));
            string res = client.RecieveCommand().MessageResponse;
            if (res.Equals("closed"))
            {
                return true;
            }
            client.Stop();

            return false;
        }
    }
}
