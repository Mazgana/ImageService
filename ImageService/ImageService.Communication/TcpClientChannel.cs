using ImageService.Communication.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ImageService.Communication
{
     public class TcpClientChannel
    {
        private TcpClient client;
        public bool IsConnected;

        private static Mutex mutex = new Mutex();
        private static TcpClientChannel instance = null;

        public event EventHandler<CommandRecievedEventArgs> UpdateModel;

        private TcpClientChannel()
        {
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8000);
            client = new TcpClient();
            try
            {
                client.Connect(ep);
                this.IsConnected = true;
                RecieveCommand();
            }
            catch(Exception e)
            {
                this.IsConnected = false;
            }
        }

        public static TcpClientChannel getInstance()
        {
            if(instance == null)
            {
                instance = new TcpClientChannel();
            }
            return instance;
        }

        public void SendCommand(CommandMessage message) {
            new Task(() =>
            {
                NetworkStream stream = client.GetStream();
                BinaryWriter writer = new BinaryWriter(stream);
                {
                    string messageInString = JsonConvert.SerializeObject(message);
                    mutex.WaitOne();
                    writer.Write(messageInString);
                    mutex.ReleaseMutex();
                }
            }).Start();
        }

        public void RecieveCommand() {
            new Task(() =>
            {
                NetworkStream stream = client.GetStream();
                BinaryReader reader = new BinaryReader(stream);
                {
                    while (true)
                    {
                        string messageInString = reader.ReadString();
                        CommandMessage message = JsonConvert.DeserializeObject<CommandMessage>(messageInString);
                        string[] args = { message.MessageResponse };
                        UpdateModel?.Invoke(this, new CommandRecievedEventArgs(message.CommandID, args, null));
                    }
                }
            }).Start();
        }

        public void Stop()
        {
            client.Close();
        }
    }
}
