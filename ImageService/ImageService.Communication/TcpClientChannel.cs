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
        BinaryReader reader;
        BinaryWriter writer;

        public TcpClientChannel()
        {
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5000);
            TcpClient client = new TcpClient();
            client.Connect(ep);
            Console.WriteLine("You are connected");
            NetworkStream stream = client.GetStream();
            reader = new BinaryReader(stream);
            writer = new BinaryWriter(stream);

            // Send data to server
            Console.Write("Please enter a number: ");
            int num = int.Parse(Console.ReadLine());

            writer.Write(num);
            // Get result from server
            int result = reader.ReadInt32();
            Console.WriteLine("Result = {0}", result);
   
            client.Close();
        }
    }
}
