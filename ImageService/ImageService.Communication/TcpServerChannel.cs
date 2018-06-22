using ImageService.Communication.Interfaces;
using ImageService.Communication.Model;
using ImageService.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ImageService.Logging.Modal;
using System.Drawing;

namespace ImageService.Communication
{
    public class TcpServerChannel
    {
        private int port;
        private TcpListener listener;
        private IClientHandler ch;
        private ILoggingService logger;
        private List<TcpClient> clients;

        public void ReceiveImage(string handler, TcpClient client)
        {
            {
                try
                {
                    NetworkStream stream = client.GetStream();
                    BinaryReader reader = new BinaryReader(stream);
                    while (true)
                    {
                        // Get result from server
                        logger.Log("recieving image..", MessageTypeEnum.INFO);
                        byte[] sizeInBytes = reader.ReadBytes(4);
                        logger.Log("got bytes", MessageTypeEnum.INFO);
                        if (BitConverter.IsLittleEndian)
                            Array.Reverse(sizeInBytes);
                        if (sizeInBytes == null)
                        {
                            return;
                        }
                        int size = BitConverter.ToInt32(sizeInBytes, 0);
                        logger.Log("size: " + size.ToString(), MessageTypeEnum.INFO);
                        byte[] message = reader.ReadBytes(size);
                        Image image = (Bitmap)((new ImageConverter()).ConvertFrom(message));
                        sizeInBytes = reader.ReadBytes(4);
                        if (BitConverter.IsLittleEndian)
                            Array.Reverse(sizeInBytes);
                        size = BitConverter.ToInt32(sizeInBytes, 0);
                        byte[] imageNameInBytes = reader.ReadBytes(size);
                        string imageName = Encoding.UTF8.GetString(imageNameInBytes, 0, imageNameInBytes.Length);
                        logger.Log("name: " + imageName, MessageTypeEnum.INFO);
                        if (Directory.Exists(handler))
                        {
                            image.Save(handler + "/" + imageName + ".jpg");
                        }
                    }
                }
                catch (IOException e)
                {
                    logger.Log("exception: " + e.ToString(), MessageTypeEnum.ERROR);
                }
                catch (ObjectDisposedException e)
                {
                    logger.Log("exception: " + e.ToString(), MessageTypeEnum.ERROR);
                }
            }
        }

        public TcpServerChannel(int port, IClientHandler ch, ILoggingService logger)
        {
            this.port = port;
            this.ch = ch;
            this.logger = logger;
            clients = new List<TcpClient>();
        }

        /// <summary>
        /// starting server connection and recieving new connections
        /// </summary>
        public void Start(string firstHandler)
        {
            //opening connectio
            IPEndPoint ep = new
            IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
            listener = new TcpListener(ep);
            listener.Start();

            logger.Log("Waiting for connections...", MessageTypeEnum.INFO);
            logger.Log("first handler: "+firstHandler, MessageTypeEnum.INFO);
            Task task = new Task(() => {
                while (true)//loop to get more connections, adding to list when client connects
                {
                    try
                    {
                        TcpClient client = listener.AcceptTcpClient();
                        this.clients.Add(client);
                        logger.Log("Got new connection", MessageTypeEnum.INFO);
                        //  ch.HandleClient(client);
                        ReceiveImage(firstHandler, client);
                        logger.Log("finished..", MessageTypeEnum.INFO);
                    }
                    catch (SocketException)
                    {
                        logger.Log("could not accept tcp client", MessageTypeEnum.ERROR);
                        break;
                    }
                }
                logger.Log("Server stopped", MessageTypeEnum.INFO);
            });
            task.Start();
        }

        private static Mutex mutex = new Mutex();

        /// <summary>
        /// sending message from server to all clients subscribed.
        /// </summary>
        /// <param name="message"> the message sent by server</param>
        public void notifyAll(CommandMessage message)
        {
            new Task(() =>
            {
                foreach (TcpClient client in clients)
                {
                    NetworkStream stream = client.GetStream();
                    BinaryWriter writer = new BinaryWriter(stream);
                    {
                        //writing message if client is connected
                        if(client.Connected)
                        {
                            string messageInString = JsonConvert.SerializeObject(message);
                            mutex.WaitOne();
                            if(clients.Contains(client))
                                writer.Write(messageInString);
                            mutex.ReleaseMutex();
                        } else
                        {
                            if (clients.Contains(client))
                                clients.Remove(client);
                        }
                    }
                }
            }).Start();

        }

        /// <summary>
        /// stopping server and closing all clients
        /// </summary>
        public void Stop()
        {
            foreach (TcpClient client in clients)
            {
                client.Close();
            }
            listener.Stop();
        }
    }
}
