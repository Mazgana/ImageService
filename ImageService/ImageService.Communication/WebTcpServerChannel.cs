using ImageService.Communication.Interfaces;
using ImageService.Communication.Model;
using ImageService.Logging;
using ImageService.Logging.Modal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Communication
{
    public class WebTcpServerChannel : IServerCommunicationChannel
    {
        private int m_port;
        private ILoggingService logger;
        private TcpListener m_listener;
        private ICollection<IClientCommunicationChannel> m_clients;
        public event EventHandler<CommandRecievedEventArgs> DataRecieved;         // The Event that notifyes that a message recieved
        public event EventHandler<NewClientEventArgs> NewClient;

        public WebTcpServerChannel(int port, ILoggingService logger)
        {
            this.m_port = port;         // Storing the Port
            //this.ch = ch;
            this.logger = logger;
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), m_port);
            this.m_listener = new TcpListener(ep);
            this.m_clients = new List<IClientCommunicationChannel>();
        }

        public bool Start()
        {
            try
            {
                m_listener.Start();

                this.logger.Log("Waiting for connections...", MessageTypeEnum.INFO);

                Task task = new Task(() =>
                {
                    IClientCommunicationChannel handler;
                    while (true)
                    {
                        try
                        {
                            TcpClient client = m_listener.AcceptTcpClient();
                            handler = new WebClintHandler(client);         // Creating the handler
                            handler.Start();                         // Starting the Recieve
                            handler.DataRecieved += Handler_DataRecieved;
                            m_clients.Add(handler);                 // Adding the Client

                            logger.Log("Got new connection", MessageTypeEnum.INFO);

                            NewClient?.Invoke(this, new NewClientEventArgs(handler));       // Notyfing that the Client Was Recieved
                        }
                        catch (SocketException)
                        {
                            logger.Log("could not accept tcp client", MessageTypeEnum.ERROR);
                            break;
                        }
                    }
                    logger.Log("Server stopped", MessageTypeEnum.INFO);
                });
                task.Start();           // Starting the Task
            }
            catch
            {
                return false;
            }
            return true;                // Return that the connection was sucessful
        }

        private void Handler_DataRecieved(object sender, CommandRecievedEventArgs e)
        {
            DataRecieved?.Invoke(sender, e);            // Forwarding the Event
        }

        public void Close()
        {
            if (m_listener != null)
            {
                m_listener.Stop();              // Stoping the Listener
            }
        }

        public void Broadcast(CommandMessage data)
        {
            IClientCommunicationChannel client;
            for (int i = m_clients.Count - 1; i >= 0; i--)
            {
                client = m_clients.ElementAt(i);

                string len = client.Send(data);

                // Trying the Send the data
                if (len.Equals("-1"))
                {
                    lock (m_clients)
                    {
                        m_clients.Remove(client);   // Removing the Client from the broadcast list
                    }
                }
            }
        }
    }
}