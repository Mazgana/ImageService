using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using ImageService.Communication.Interfaces;
using ImageService.Logging;

namespace ImageService.Communication
{
    public class ClientHandler : IClientHandler
    {
        NetworkStream stream;
        StreamReader reader;
        StreamWriter writer;

        public void HandleClient(TcpClient client, ILoggingService logg)
        {
            new Task(() =>
            {
                this.stream = client.GetStream();
                this.reader = new StreamReader(stream);
                this.writer = new StreamWriter(stream);
                {
                    writer.AutoFlush = true;
                    //   writer.WriteLine("maybe if i write!");
                    //   writer.WriteLine("maybe if i write 2222!");
                    logg.Log("waiting for message from client", Logging.Modal.MessageTypeEnum.INFO);
                    string commandLine = reader.ReadLine();
                    logg.Log("Got command", Logging.Modal.MessageTypeEnum.INFO);
                    logg.Log(commandLine, Logging.Modal.MessageTypeEnum.INFO);
                    string commandLine2 = reader.ReadLine();
                    logg.Log("Got command2", Logging.Modal.MessageTypeEnum.INFO);
                    logg.Log(commandLine2, Logging.Modal.MessageTypeEnum.INFO);
                    string commandLine3 = reader.ReadLine();
                    logg.Log("Got command3", Logging.Modal.MessageTypeEnum.INFO);
                    logg.Log(commandLine3, Logging.Modal.MessageTypeEnum.INFO);
                    // string result = ExecuteCommand(commandLine, client);
                    writer.WriteLine("Command executed!\n");
                    logg.Log("finished task", Logging.Modal.MessageTypeEnum.INFO);
                }
                client.Close();
            }).Start();
        }

        public void sendLine(string line)
        {
            this.writer.WriteLine(line);
        }
    }
}
