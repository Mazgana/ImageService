using ImageService.Communication;
using ImageService.Communication.Interfaces;
using ImageService.Communication.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace leah
{
    class Debugger
    {
        public Debugger()
        {
            Console.WriteLine("start debugger\n");
            TcpClientChannel client = TcpClientChannel.getInstance();
            Console.WriteLine("created tcp client");
            client.UpdateModel += ViewUpdate;
            if (client.IsConnected)
            {
                client.SendCommand(new CommandMessage(3, null));
                Console.WriteLine("sent message 3 to server");
                client.SendCommand(new CommandMessage(2 ,null));
                Console.WriteLine("sent message 2 to server");

                System.Threading.Thread.Sleep(1000);
                client.SendCommand(new CommandMessage(2, null));
                Console.WriteLine("sent message 2 to server");

                /*
                CommandMessage result = client.RecieveCommand();
                Console.WriteLine("got message number: " + result.ID);
                List<String> message = JsonConvert.DeserializeObject<List<String>>(result.MessageResponse);

                foreach (String st in message)
                {
                    Console.WriteLine("got entry:");
                    Console.WriteLine(st);
                    Console.WriteLine("------------");
                }
                */

                //while (true)
                //{
                //    client.SendCommand(new CommandMessage(5, null));
                //    Console.WriteLine("sent message please update to server");
                System.Threading.Thread.Sleep(1000000);
                //}
            }
            else
            {
                Console.WriteLine("could not connect to server");
            }
            //   client.Stop();
            System.Threading.Thread.Sleep(1000);
            Console.WriteLine("end");
        }

        private void ViewUpdate(object sender, CommandRecievedEventArgs e)
        {
            Console.WriteLine("got message from server number: " + e.CommandID);
        }
    }
}
