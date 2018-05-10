using ImageService.Communication.Interfaces;
using ImageService.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Communication
{
    public class TcpServerChannel
    {
        private int port;
        private TcpListener listener;
        private IClientHandler ch;
        ILoggingService logger;

        public TcpServerChannel(int port, IClientHandler ch)
        {
            this.port = port;
            this.ch = ch;
//            this.logger = ls;
        }

        public void Start()
        {
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
            listener = new TcpListener(ep);

            listener.Start();
//            logger.Log("Waiting for connections...", Logging.Modal.MessageTypeEnum.INFO);
            Task task = new Task(() => {
                while (true)
                {
                    try
                    {
                        TcpClient client = listener.AcceptTcpClient();
//                        logger.Log("Got new connection", Logging.Modal.MessageTypeEnum.INFO);
                        ch.HandleClient(client);
                    }
                    catch (SocketException)
                    {
                        break;
                    }
                }

 //               logger.Log("Server stopped", Logging.Modal.MessageTypeEnum.WARNING);
            });
            task.Start();
        }

        public void Stop()
        {
            listener.Stop();
        }
    }
}
