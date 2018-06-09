using ImageService.Communication.Interfaces;
using ImageService.Infrastructure.Event;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ImageService.Communication
{
    public class SynchTcpClientHandler : IClientCommunicationChannel
    {
        #region Members
        private TcpClient m_client;         // The Client Instance
        private NetworkStream m_stream;     // The Stream To Close The Stream
        private StreamReader m_reader;            // The Reader
        private StreamWriter m_writer;            // The Writer

        #endregion

        public SynchTcpClientHandler(TcpClient client)
        {
            m_client = client;
            m_stream = client.GetStream();
            m_reader = new StreamReader(m_stream, Encoding.ASCII);
            m_writer = new StreamWriter(m_stream, Encoding.ASCII);
        }
        public event EventHandler<DataRecievedEventArgs> DataRecieved;

        // The Function Closes The Handler
        public void Close()
        {
            DisposeHandler();         // Closing the channel
        }

        public string Send(string data)
        {
            try
            {
                m_writer.WriteLine(Regex.Replace(data, @"\t|\n|\r", ""));           // Writing the Data to the Client
                m_writer.Flush();                   // Sending the Line
                return m_reader.ReadLine();      // Getting the string from the client;                 // return the Length of the Length
            }
            catch (Exception e)
            {
                DisposeHandler();               // Closing the Handler
                return "";
            }
        }

        // Starting to Recieve Data
        public bool Start()
        {
            return true;
        }

        private void DisposeHandler()
        {
            m_writer.Close();               // Closing the Writer
            m_reader.Close();               // Closing the Reader
            m_stream.Close();               // Closing the Straem
            m_client.Close();               // Closing the Client
        }
    }
}
