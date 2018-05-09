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
    public class CommunicationServer
    {
        private static CommunicationServer instance;
        private int port;
        private TcpListener listener;
        private IClientHandler ch;

        private CommunicationServer()
        {
            // Read the file and display it line by line.  
            System.IO.StreamReader file =
            new System.IO.StreamReader(@"ServerConfiguration.txt");
            string line = file.ReadLine();

            this.port = Int32.Parse(line.Substring(6, line.Length - 1));
            file.Close();
        }

        public static CommunicationServer Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new CommunicationServer();
                }
                return instance;
            }
        }

        public void Start()
        {
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
            listener = new TcpListener(ep);

            listener.Start();
            Console.WriteLine("Waiting for connections...");

            Task task = new Task(() =>
            {
                while (true)
                {
                    try
                    {
                        TcpClient client = listener.AcceptTcpClient();
                        Console.WriteLine("Got new connection");
                        ch.HandleClient(client);
                    }
                    catch (SocketException)
                    {
                        break;
                    }
                }
                Console.WriteLine("Server stopped");
            });
            task.Start();
        }

        public void Stop()
        {
            listener.Stop();
        }
    }
}
