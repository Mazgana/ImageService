using ImageService.Communication.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Communication.Interfaces
{
    public interface ICommunicationChannel
    {
        event EventHandler<CommandRecievedEventArgs> DataRecieved;         // The Event that notifyes that a message recieved
        void Close();                                            // Cloing the Channel
        bool Start();                                            // Starting the Channel
    }
}
