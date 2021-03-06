﻿using ImageService.Communication;
using ImageService.Communication.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Web;

namespace WebApplication2.Models
{
    public class Communication
    {
        TcpClientChannel Client { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "IsConnected")]
        public bool IsConnected { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "ServiceConfig")]
        public Config ServiceConfig { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "CloseHandler")]
        public string CloseHandler { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "LogList")]
        public ObservableCollection<Log> LogList { get; set; }

        String currentLog;
        bool gotLog;
        bool gotConfig;
        bool handlerDeleted;

        public Communication()
        {
            this.ServiceConfig = new Config();
            this.LogList = new ObservableCollection<Log>();

            this.gotConfig = false;
            this.gotLog = false;

            this.Client = TcpClientChannel.getInstance();

            if (this.Client.IsConnected)
            {
                this.IsConnected = true;
                this.Client.UpdateModel += ViewUpdate;

                this.Client.SendCommand(new CommandMessage(3, null));
                while (!this.gotLog)
                {
                    Thread.Sleep(1000);
                }

                this.Client.SendCommand(new CommandMessage(2, null));
                while (!this.gotConfig)
                {
                    Thread.Sleep(1000);
                }
            }
        }

        public bool RemoveHandler(String handler)
        {
            TcpClientChannel RemoveClient = TcpClientChannel.getInstance();

            RemoveClient.SendCommand(new CommandMessage(4, handler));
            while (!this.handlerDeleted)
            {
                Thread.Sleep(1000);
            }

            ServiceConfig.Handlers.Remove(handler);
            RemoveClient.Stop();

            return true;
        }

        private void ViewUpdate(object sender, CommandRecievedEventArgs e)
        {
            //Checks if the server respose is the application config.
            if (e.CommandID == 2)
            {
                //initialize the settings window's members to hold the configuration values.
                string config = e.Args[0];
                string[] configSrtings = config.Split('|');

                this.gotConfig = true;

                this.ServiceConfig.Handlers = new List<string>();

                this.ServiceConfig.OutputDirectory = configSrtings[0];
                this.ServiceConfig.SourceName = configSrtings[1];
                this.ServiceConfig.LogName = configSrtings[2];
                this.ServiceConfig.ThumbSize = configSrtings[3];

                string[] handlersDirectories = configSrtings[4].Split(';');
                for (int i = 0; i < handlersDirectories.Length; i++)
                {
                    if (handlersDirectories[i].Length != 0)
                        this.ServiceConfig.Handlers.Add(handlersDirectories[i]);
                }


            } else if (e.CommandID == 3 && LogList.Count < 2) {
                this.currentLog = e.Args[0];
                this.gotLog = true;

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
            }

            //checks if the server's response is the close handler response. if it is - updates the handlers list.
            if (e.CommandID == 4)
            {
                handlerDeleted = true;
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