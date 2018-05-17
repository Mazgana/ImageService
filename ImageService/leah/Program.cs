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
            client.SendCommand(new ImageService.Communication.Model.CommandMessage(0, "just trying"));
            Console.WriteLine("end");
            client.Stop();
        }
    }
}
