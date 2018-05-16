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

namespace ImageService.Communication
{
    public class TcpServerChannel
    {
        private int port;
        private TcpListener listener;
        private IClientHandler ch;
        private ILoggingService logger;
        private List<TcpClient> clients;

        public TcpServerChannel(int port, IClientHandler ch, ILoggingService logger)
        {
            this.port = port;
            this.ch = ch;
            this.logger = logger;
            clients = new List<TcpClient>();
        }

        public void Start()
        {
            IPEndPoint ep = new
            IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
            listener = new TcpListener(ep);
            listener.Start();

            logger.Log("Waiting for connections...", Logging.Modal.MessageTypeEnum.INFO);
            Task task = new Task(() => {
                while (true)
                {
                    try
                    {
                        TcpClient client = listener.AcceptTcpClient();
                        this.clients.Add(client);
                        logger.Log("Got new connection",Logging.Modal.MessageTypeEnum.INFO);
                        ch.HandleClient(client, logger);
                    }
                    catch (SocketException)
                    {
                        logger.Log("could not accept tcp client", Logging.Modal.MessageTypeEnum.FAIL);
                        break;
                    }
                }
                logger.Log("Server stopped", Logging.Modal.MessageTypeEnum.INFO);
            });
            task.Start();
        }

        private static Mutex mutex = new Mutex();

        public void notifyAll(CommandMessage message)
        {

            new Task(() =>
            {
                foreach (TcpClient client in clients)
                {
                    NetworkStream stream = client.GetStream();
                    BinaryWriter writer = new BinaryWriter(stream);
                    {
                        
                        string messageInString = JsonConvert.SerializeObject(message);
                        mutex.WaitOne();
                        writer.Write(messageInString);
                        mutex.ReleaseMutex();
                    }
                }

            }).Start();

        }

        public void Stop()
        {
            listener.Stop();
        }
    }
}
