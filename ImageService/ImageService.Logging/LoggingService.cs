using ImageService.Logging.Modal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Logging
{
    public class LoggingService : ILoggingService
    {
        List<string> fullLog = new List<string>();

        public event EventHandler<MessageRecievedEventArgs> MessageRecieved;

        /// <summary>
        /// Adding the log registers the given message.
        /// </summary>
        /// <param name="message"> The message text </param>
        /// <param name="type"> The message type - information, warning etc. </param>
        public void Log(string message, MessageTypeEnum type)
        {
            string msg = message + "|"+ type;
            fullLog.Add(msg);
            MessageRecieved?.Invoke(this, new MessageRecievedEventArgs(type, message));
        }
    }
}
