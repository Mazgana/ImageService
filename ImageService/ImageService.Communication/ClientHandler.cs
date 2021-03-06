﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
//using ImageService.Modal.Events;
using ImageService.Communication.Interfaces;
using ImageService.Logging;
using ImageService.Communication.Model;
using System.Drawing;
using ImageService.Logging.Modal;

namespace ImageService.Communication
{
    /// <summary>
    /// handles all command recieved from client
    /// </summary>
    public class ClientHandler : IClientHandler
    {
        public event EventHandler<CommandRecievedEventArgs> CommandRecieved;
        /// <summary>
        /// reading messages from client and sending them so service
        /// </summary>
        /// <param name="client"> sendind the message </param>

        public void HandleClient(TcpClient client)
        {
            new Task(() =>
            {
                using (NetworkStream stream = client.GetStream())
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    while (true)
                    {
                        if (client.Connected)
                        {
                            string messageInString = reader.ReadString();//reading string message
                            CommandMessage message = JsonConvert.DeserializeObject<CommandMessage>(messageInString);
                            string[] args = { message.MessageResponse };
                            CommandRecieved?.Invoke(this, new CommandRecievedEventArgs(message.CommandID, args, message.MessageResponse));//sending message to server
                        }
                    }
                }
            }).Start();
        }
    }
}
