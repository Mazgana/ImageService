using ImageService.Communication;
using ImageService.Communication.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;

namespace WebApplication2.Models
{
    public class Communication
    {
        TcpClientChannel Client { get; set; }
        public bool IsConnected { get; set; }

        public Config ServiceConfig { get; set; }
        public List<Log> LogList { get; set; }
        String currentLog;
        bool gotLog;

        public Communication()
        {
            this.ServiceConfig = new Config();
            this.LogList = new List<Log>();

            this.Client = TcpClientChannel.getInstance();

            if (this.Client.IsConnected)
            {
                this.IsConnected = true;
                this.Client.UpdateModel += ViewUpdate;
                this.Client.SendCommand(new ImageService.Communication.Model.CommandMessage(2, null));
            }
        }

        private void ViewUpdate(object sender, CommandRecievedEventArgs e)
        {
            //Checks if the server respose is the application config.
            if (e.CommandID == 2)
            {

                //initialize the settings window's members to hold the configuration values.
                string config = e.Args[0];
                string[] configSrtings = config.Split('|');

                ServiceConfig.OutputDirectory = configSrtings[0];
                ServiceConfig.SourceName = configSrtings[1];
                ServiceConfig.LogName = configSrtings[2];
                ServiceConfig.ThumbSize = configSrtings[3];

                string[] handlersDirectories = configSrtings[4].Split(';');
                for (int i = 0; i < handlersDirectories.Length; i++)
                {
                    if (handlersDirectories[i].Length != 0)
                        ServiceConfig.Handlers.Add(handlersDirectories[i]);
                }
            } else if (e.CommandID == 3 && LogList.Count < 2) {
                this.currentLog = e.Args[0];
                List<string> allLog = JsonConvert.DeserializeObject<List<String>>(this.currentLog);
                string[] current;

                foreach (String st in allLog)
                {
                    current = st.Split('|');
                    Log curr = new Log
                    {
                        Type = current[0],
                        Message = current[1]
                    };
                    this.LogList.Insert(0, curr);
                }
                gotLog = true;
            }

            //Checks if the recieved data is relevant to the log view and if its message from the current running
            // that was recieved in real time.
            if (e.CommandID == 5 && IsConnected && gotLog)
            {
                string[] mes;
                string entry = e.Args[0];
                mes = e.Args[0].Split('|');
                Log curr = new Log
                {
                    Type = mes[0],
                    Message = mes[1]
                };
                this.LogList.Insert(0, curr);
            }
        }
    }
}