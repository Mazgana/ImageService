using ImageService.Logging;
using ImageService.Modal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Configuration;

namespace ImageService.Commands
{
    class LogCommand : ICommand
    {
        ILoggingService logging;

        /// <summary>
        /// Constructor. saves modal as member.
        /// </summary>
        /// <param name="modal"> The modal that will make the command operations. </param>
        public LogCommand()
        {
            //logging = logger;
        }

        /// <summary>
        /// executes command to send the log of the application.
        /// </summary>
        /// <param name="args"> string arguments that is empty. </param>
        /// <returns> The return value is all the app config file properties concat to one stirng, 
        /// and will return the error message. </returns>
        public string Execute(string[] args, out bool result)
        {
            string messageInString = null;
            result = true;
            try
            {
                List<String> fullLog = new List<String>();

                EventLog eventLog = new EventLog();
                eventLog.Log = ConfigurationManager.AppSettings["LogName"];
                eventLog.Source = ConfigurationManager.AppSettings["SourceName"];
                //eventLog.MachineName = machineName;

                foreach (EventLogEntry log in eventLog.Entries)
                {
                    String entry = log.EntryType.ToString() + "|" + log.Message;
                    fullLog.Add(entry);
                }
                messageInString = JsonConvert.SerializeObject(fullLog);
            } catch(Exception e)
            {
                result = false;
            }
            return messageInString;
        }
    }
}
