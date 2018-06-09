using ImageService.Communication.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Text.RegularExpressions;
using ImageService.Communication.Model;
using Newtonsoft.Json;

namespace ImageService.Communication
{
    public class WebClintHandler : IClientCommunicationChannel
    {
        #region Members
        private TcpClient m_client;         // The Client Instance
        private NetworkStream m_stream;     // The Stream To Close The Stream
        private StreamReader m_reader;            // The Reader
        private StreamWriter m_writer;            // The Writer

        private CancellationTokenSource m_cancelToken;          // The Cancelation Token
        #endregion

        public WebClintHandler(TcpClient client)
        {
            m_client = client;
            m_stream = client.GetStream();
            m_reader = new StreamReader(m_stream, Encoding.ASCII);
            m_writer = new StreamWriter(m_stream, Encoding.ASCII);
            m_cancelToken = new CancellationTokenSource();
        }
        public event EventHandler<CommandRecievedEventArgs> DataRecieved;

        // The Function Closes The Handler
        public void Close()
        {
            m_cancelToken.Cancel();         // Canceling the Recieve
        }

        public string Send(CommandMessage data)
        {
            try
            {
                m_writer.WriteLine(Regex.Replace(data.MessageResponse, @"\t|\n|\r", ""));           // Writing the Data to the Client
                m_writer.Flush();                   // Sending the Line
                return data.MessageResponse.Length.ToString();                 // return the Length of the Length
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
            string messageInString;
            new Task(() =>
            {
                try
                {
                    while (true)
                    {
                        messageInString = m_reader.ReadLine();      // Getting the string from the client

                        if (messageInString != null)
                        {
                            CommandMessage message = JsonConvert.DeserializeObject<CommandMessage>(messageInString);
                            string[] args = { message.MessageResponse };
                            DataRecieved?.Invoke(this, new CommandRecievedEventArgs(message.CommandID, args, null));//passing message to client
                        }
                    }
                }
                catch (Exception e)
                {
                    DisposeHandler();               // Closing the Handler

                }
            }, m_cancelToken.Token).Start();
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
