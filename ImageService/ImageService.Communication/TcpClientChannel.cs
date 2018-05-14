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
     public class TcpClientChannel
    {
        public TcpClientChannel()
        {
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8000);
            TcpClient client = new TcpClient();
            client.Connect(ep);
            Console.WriteLine("You are connected");
            using (NetworkStream stream = client.GetStream())
            using (StreamReader reader = new StreamReader(stream))
            using (StreamWriter writer = new StreamWriter(stream))
            {
                writer.AutoFlush = true;
               // string resulttry = reader.ReadLine();
               // Console.WriteLine(" look what i got: " + resulttry);
                // Send data to server
                writer.WriteLine("sending message1");
                writer.WriteLine("sending message2");
                writer.WriteLine("sending message3");
                // Get result from server
                Console.WriteLine("all messages were sent");
                
                string result = reader.ReadLine();
                Console.WriteLine("Got message: ");
                Console.WriteLine(result);
            }
            client.Close();

        }
    }
}
