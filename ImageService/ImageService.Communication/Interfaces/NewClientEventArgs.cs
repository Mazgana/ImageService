using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Communication.Interfaces
{
    public class NewClientEventArgs : EventArgs
    {
        public IClientCommunicationChannel Client { get; set; }

        public NewClientEventArgs(IClientCommunicationChannel a_client)
        {
            Client = a_client;
        }
    }
}