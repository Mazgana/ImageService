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
        public void HandleAppClient(TcpClient client, String handler, ILoggingService logger)
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
                                // NetworkStream stream = client.GetStream();
                                //byte[] data = new byte[4];
                                logger.Log("reading size..", MessageTypeEnum.INFO);
                                //Console.WriteLine("reading size");
                                //Read The Size
                                //reader.Read(data, 0, data.Length);

                                byte[] data = reader.ReadBytes(4);

                                if (data == null)
                                    return;

                                int size = BitConverter.ToInt32(data, 0);
                                // prepare buffer
                                //data = new byte[size];
                                //Console.WriteLine("size is: " + size.ToString() + "reading image");
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
                                //Console.WriteLine("finished while..");
                                byte[] imageNameInBytes = reader.ReadBytes(size);
                                //read = 0;
                                //while (read != size)
                                //{
                                //    read += reader.Read(imageNameInBytes, read, data.Length - read);
                                //}

                                string imgName = Encoding.UTF8.GetString(imageNameInBytes, 0, imageNameInBytes.Length);
                                logger.Log("got name..", MessageTypeEnum.INFO);
                                //Console.WriteLine("got name");
                                //stream.Read(data, 0, data.Length);

                                //Convert Image Data To Image
                                MemoryStream imagestream = new MemoryStream(data);

                                img.Save(handler + "/" + imgName);

                                //System.Drawing.Bitmap bmp = new Bitmap(imagestream);
                                //bmp.Save(handler, System.Drawing.Imaging.ImageFormat.Png);
                                logger.Log("saved image..", MessageTypeEnum.INFO);
                                //Console.WriteLine("saved image");
                                // pictureBox1.Image = bmp;
                        }
                    }
                }
            }).Start();
        }

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
