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

        public MsgRecievedEventArgs(string msgType, string msgText)
        {
            this.type = msgType;
            this.text = msgText;
        }
    }
}
