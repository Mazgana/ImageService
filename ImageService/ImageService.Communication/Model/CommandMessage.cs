using ImageService.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Communication.Model
{
    public class CommandMessage
    {
        //public CommandEnum CommandType { get; set; }
        public int CommandID { get; set; }
        public string MessageResponse { get; set; }

        /// <summary>
        /// Constractor for command message.
        /// </summary>
        /// <param name="type"> the command type </param>
        /// <param name="mes"> the response from the execution of the command</param>
        public CommandMessage(int id, string mes)
        {
            this.CommandID = id;
            this.MessageResponse = mes;
        }
    }
}
