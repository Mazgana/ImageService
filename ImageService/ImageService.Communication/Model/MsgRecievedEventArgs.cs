using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Communication.Model
{
    public class MsgRecievedEventArgs
    {
        private int msgID;
        private string msgText;

        public MsgRecievedEventArgs(int id, string msg)
        {
            this.msgID = id;
            this.msgText = msg;
        }
        
        public int id
        {
            get { return msgID; }
            set { msgID = value; }
        }

        public string msg
        {
            get { return msgText; }
            set { msgText = value; }
        }
    }
}
