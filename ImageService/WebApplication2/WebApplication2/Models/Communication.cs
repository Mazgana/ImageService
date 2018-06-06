using ImageService.Communication;
using ImageService.Communication.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication2.Models
{
    public class Communication
    {
        TcpClientChannel client { get; set; }

        public Config serviceConfig { get; set; }

        public Communication()
        {
            serviceConfig = new Config();

            this.client = TcpClientChannel.getInstance();
            this.client.UpdateModel += ViewUpdate;
            this.client.SendCommand(new ImageService.Communication.Model.CommandMessage(2, null));
        }

        private void ViewUpdate(object sender, CommandRecievedEventArgs e)
        {
            //Checks if the server respose is the application config.
            if (e.CommandID == 2)
            {

                //initialize the settings window's members to hold the configuration values.
                string config = e.Args[0];
                string[] configSrtings = config.Split('|');

                serviceConfig.OutputDirectory = configSrtings[0];
                serviceConfig.SourceName = configSrtings[1];
                serviceConfig.LogName = configSrtings[2];
                serviceConfig.ThumbSize = configSrtings[3];

                string[] handlersDirectories = configSrtings[4].Split(';');
                //for (int i = 0; i < handlersDirectories.Length; i++)
                //{
                //    if (handlersDirectories[i].Length != 0)
                //        serviceConfig.Handlers.Add(handlersDirectories[i]);
                //}
            }
        }
    }
}