using ImageService.Communication.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Communication.Interfaces
{
    // The Interface sets the basic functions for a communication Channel
    public interface IServerCommunicationChannel : ICommunicationChannel
    {
        event EventHandler<NewClientEventArgs> NewClient;
        void Broadcast(CommandMessage data);        // Broadcasting the Data to all Clients
    }
}
