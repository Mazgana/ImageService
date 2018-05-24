using ImageService.Communication;
using ImageService.Communication.Model;
using Newtonsoft.Json;
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
            /*
            TcpClientChannel client = new TcpClientChannel();
            client.SendCommand(new ImageService.Communication.Model.CommandMessage(3, "getLog"));
            CommandMessage config = client.RecieveCommand();
            Console.WriteLine("got entry:");
            Console.WriteLine(config.MessageResponse);

            */

            TcpClientChannel client = TcpClientChannel.getInstance();
            Console.WriteLine("created tcp client");
            client.SendCommand(new CommandMessage(3, null));
            Console.WriteLine("sent message");
            CommandMessage result = client.RecieveCommand();
            Console.WriteLine("got message");
             List<String> message = JsonConvert.DeserializeObject<List<String>>(result.MessageResponse);

             foreach (String st in message) {
                 Console.WriteLine("got entry:");
                 Console.WriteLine(st);
                Console.WriteLine("------------");
             }
            client.Stop();
        }
    }
}
