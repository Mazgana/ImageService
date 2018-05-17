using System;
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

namespace ImageService.Communication
{
    public class ClientHandler : IClientHandler
    {
        public event EventHandler<CommandRecievedEventArgs> CommandRecieved;

        public void HandleClient(TcpClient client, ILoggingService logg)
        {
            new Task(() =>
            {
                using (NetworkStream stream = client.GetStream())
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    while (true)
                    {
                        logg.Log("waiting for message from client", Logging.Modal.MessageTypeEnum.INFO);
                        string messageInString = reader.ReadString();
                        CommandMessage message = JsonConvert.DeserializeObject<CommandMessage>(messageInString);
                        logg.Log("Got command", Logging.Modal.MessageTypeEnum.INFO);
                        string[] args = { };
                        CommandRecieved?.Invoke(this, new CommandRecievedEventArgs(message.CommandID, args, message.MessageResponse));
                    }
                }
                client.Close();
            }).Start();
        }
    }
}
