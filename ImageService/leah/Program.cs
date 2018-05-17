using ImageService.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace leah
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("main\n");
            TcpClientChannel client = new TcpClientChannel();
            Console.WriteLine("created tcp client");
            client.SendCommand(new ImageService.Communication.Model.CommandMessage(4, "C:\\Users\\as\\Desktop\\pics"));
            Console.WriteLine("sent message");
            client.RecieveCommand();
            Console.WriteLine("end. got message");
            client.Stop();
        }
    }
}
