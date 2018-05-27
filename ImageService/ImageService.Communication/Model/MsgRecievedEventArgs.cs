using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Communication.Model
{
    public class MsgRecievedEventArgs
    {
        public string type { get; set; }
        public string text { get; set; }

        /// <summary>
        /// holds details of type and string message for passing to log
        /// </summary>
        /// <param name="msgType"> message type </param>
        /// <param name="msgText"> message string </param>
        public MsgRecievedEventArgs(string msgType, string msgText)
        {
            this.type = msgType;
            this.text = msgText;
        }
    }
}
