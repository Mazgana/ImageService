using ImageService.Communication.Interfaces;
using ImageService.Communication.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Communication
{
    class WebTcpClientChannel : IClientCommunicationChannel
    {
        //private int m_port;
        //private string m_ip;                // The IP Address
        private TcpClient m_client;         // the Client Connection
        private IClientCommunicationChannel m_handler;       // The Handler of the 

        public event EventHandler<CommandRecievedEventArgs> DataRecieved;

        public WebTcpClientChannel(string a_ip, int a_port)
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

                m_handler = new TcpClientHandler(m_client);     // Creating the Client Handler
                m_handler.DataRecieved += OnDataRecieved;
                m_handler.Start();
            }
            catch
            {
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

        public string Send(string data)
        {
            if (m_handler != null)
            {
                return m_handler.Send(data);           // Sending the Data via the handler
            }
            return "";
        }

        #endregion

        // The function that will be activated upon data recieved
        private void OnDataRecieved(object sender, CommandRecievedEventArgs e)
        {
            DataRecieved?.Invoke(sender, e);
        }

    }
}