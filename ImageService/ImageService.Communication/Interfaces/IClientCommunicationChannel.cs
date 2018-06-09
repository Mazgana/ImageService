using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Communication.Interfaces
{
    public interface IClientCommunicationChannel : ICommunicationChannel
    {
        string Send(string data);                                 // The Function that sends a response
    }
}
