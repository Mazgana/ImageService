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
using System.Collections.ObjectModel;

namespace ImageService.Commands
{
    class LogCommand : ICommand
    {

        /// <summary>
        /// Constructor. saves modal as member.
        /// </summary>
        /// <param name="modal"> The modal that will make the command operations. </param>
        public LogCommand()
        {
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
                //opening event log for reading entries
                EventLog eventLog = new EventLog();
                eventLog.Log = ConfigurationManager.AppSettings["LogName"];
                eventLog.Source = ConfigurationManager.AppSettings["SourceName"];
                
                //reading all entries in log history
                foreach (EventLogEntry log in eventLog.Entries)
                {
                    string type = log.EntryType.ToString();
                    if (type.Equals("Information"))
                        type = "INFO";
                    else if (type.Equals("Warning"))
                        type = "WARNING";
                    else if (type.Equals("FailureAudit"))
                        type = "ERROR";

                    String entry = type + "|" + log.Message;
                    fullLog.Add(entry);
                }
                //return list as string
                messageInString = JsonConvert.SerializeObject(fullLog);
            } catch(Exception e)
            {
                Console.WriteLine(e.ToString());
                result = false;
            }
            return messageInString;
        }
    }
}
