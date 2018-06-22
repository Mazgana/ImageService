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
                catch (Exception)
                {
                    logger.Log("could not read image",MessageTypeEnum.ERROR);
                }
            }
            /*
            logger.Log("recieving image..",MessageTypeEnum.INFO);
            //    foreach (TcpClient client in clients)
            //    {
            new Task(() =>
            {
                NetworkStream stream = client.GetStream();
                BinaryReader reader = new BinaryReader(stream);
  
                    while (client.Connected)
                    {

                        {
                            try
                            {
                                // NetworkStream stream = client.GetStream();
                                //byte[] data = new byte[4];
                                logger.Log("reading size..", MessageTypeEnum.INFO);
                                //Read The Size
                                //reader.Read(data, 0, data.Length);

                                byte[] data = reader.ReadBytes(4);
                                
                                if (BitConverter.IsLittleEndian)
                                    Array.Reverse(data);
                                if (data == null)
                                {
                                    return;
                                }

                                int size = BitConverter.ToInt32(data, 0);
                                // prepare buffer
                                //data = new byte[size];
                                logger.Log("size is: " + size.ToString() + "reading image", MessageTypeEnum.INFO);
                                //Load Image
                                //int read = 0;
                                //while (read != size)
                                //{
                                //    read += reader.Read(data, read, data.Length - read);
                                //}

                                data = new byte[size];
                                data = reader.ReadBytes(size);

                                Image img = (Bitmap)((new ImageConverter()).ConvertFrom(data));

                                //read the image's name
                                byte[] lengthOfName = reader.ReadBytes(4);

                                //reader.Read(data, 0, data.Length);
                                size = (BitConverter.ToInt32(lengthOfName, 0));
                                logger.Log("finished while..", MessageTypeEnum.INFO);
                                byte[] imageNameInBytes = reader.ReadBytes(size);
                                //read = 0;
                                //while (read != size)
                                //{
                                //    read += reader.Read(imageNameInBytes, read, data.Length - read);
                                //}

                                string imgName = Encoding.UTF8.GetString(imageNameInBytes, 0, imageNameInBytes.Length);
                                logger.Log("finished while..", MessageTypeEnum.INFO);
                                //stream.Read(data, 0, data.Length);

                                //Convert Image Data To Image
                                MemoryStream imagestream = new MemoryStream(data);

                                img.Save(handler + "/" + imgName);

                                //System.Drawing.Bitmap bmp = new Bitmap(imagestream);
                                //bmp.Save(handler, System.Drawing.Imaging.ImageFormat.Png);
                                logger.Log("saved image..", MessageTypeEnum.INFO);
                                // pictureBox1.Image = bmp;
                            }
                            catch(Exception e)
                            {
                                logger.Log("image could not be read, exception: "+ e.Message, MessageTypeEnum.INFO);
                            }
                        }
                    }
                
                // }
            }).Start();
            */
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
                        //ch.HandleAppClient(client, firstHandler);
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
