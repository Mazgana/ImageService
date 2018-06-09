using ImageService.Communication.Interfaces;
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
    public class WebTcpClientChannel : IClientCommunicationChannel
    {
        //private int m_port;
        //private string m_ip;                // The IP Address
        private TcpClient m_client;         // the Client Connection
        private IClientCommunicationChannel m_handler;       // The Handler of the 

        //private TcpClient client;
        public bool IsConnected;

        private static Mutex mutex = new Mutex();
        //private static WebTcpClientChannel instance = null;

        public event EventHandler<CommandRecievedEventArgs> DataRecieved;

        public WebTcpClientChannel()
        {
            //this.m_port = a_port;         // Storing the Porte
            //this.m_ip = a_ip;           // Storing the IP Of the channel
            m_handler = null;           // Setting that the Handler doesn't exist
        }

        #region IClientHandler  ICommunicationChannel
        // Starting the Connection With The Server
        public bool Start()
        {
            try
            {
                IPEndPoint ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8000);
                m_client = new TcpClient();       // Creating the Client Connection
                m_client.Connect(ep);             // Trying to connect to the Server

                IsConnected = true;

                m_handler = new WebClintHandler(m_client);     // Creating the Client Handler
                m_handler.DataRecieved += OnDataRecieved;
                m_handler.Start();
            }
            catch
            {
                IsConnected = false;
                return false;
            }
            return true;                // Return that the connection was sucessful
        }

        // Stoping the Server
        public void Close()
        {
            if (m_client != null)
            {
                m_client.Close();              // Closing the Connection
            }
            if (m_handler != null)
            {
                m_handler.Close();
            }
        }

        public string Send(CommandMessage message)
        {
            if (m_handler != null)
            {
                return m_handler.Send(message);           // Sending the Data via the handler
            }
            return "";
        }

        //public void SendCommand(CommandMessage message)
        //{
        //    new Task(() =>
        //    {
        //        NetworkStream stream = m_client.GetStream();
        //        BinaryWriter writer = new BinaryWriter(stream);
        //        {
        //            //serializing message and sending it
        //            string messageInString = JsonConvert.SerializeObject(message);
        //            mutex.WaitOne();
        //            writer.Write(messageInString);
        //            mutex.ReleaseMutex();
        //        }
        //    }).Start();
        //}

        #endregion

        // The function that will be activated upon data recieved
        private void OnDataRecieved(object sender, CommandRecievedEventArgs e)
        {
            new Task(() =>
            {
                NetworkStream stream = this.m_client.GetStream();
                BinaryReader reader = new BinaryReader(stream);
                {
                    while (true)
                    {
                        string messageInString = reader.ReadString();
                        CommandMessage message = JsonConvert.DeserializeObject<CommandMessage>(messageInString);
                        string[] args = { message.MessageResponse };
                        DataRecieved?.Invoke(this, new CommandRecievedEventArgs(message.CommandID, args, null));//passing message to client
                    }
                }
            }).Start();
        }

        //public static WebTcpClientChannel getInstance()
        //{
        //    if (instance == null)
        //    {
        //        instance = new WebTcpClientChannel();
        //    }
        //    return instance;
        //}

    }
}