using ImageService.Modal;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Commands
{
    class GetConfigCommand : ICommand
    {

        /// <summary>
        /// Constructor. saves modal as member.
        /// </summary>
        /// <param name="modal"> The modal that will make the command operations. </param>
        public GetConfigCommand()
        {
        }

        /// <summary>
        /// executes command to send the app config properties.
        /// </summary>
        /// <param name="args"> string arguments that is empty. </param>
        /// <returns> The return value is all the app config file properties concat to one stirng, 
        /// and will return the error message. </returns>
        public string Execute(string[] args, out bool result)
        {
            
            string config = ConfigurationManager.AppSettings["OutputDir"];
            config = String.Concat(config, "|");
            config = String.Concat(config, ConfigurationManager.AppSettings["SourceName"]);
            config = String.Concat(config, "|");
            config = String.Concat(config, ConfigurationManager.AppSettings["LogName"]);
            config = String.Concat(config, "|");
            config = String.Concat(config, ConfigurationManager.AppSettings["ThumbnailSize"]);
            config = String.Concat(config, "|");
            config = String.Concat(config, ConfigurationManager.AppSettings["Handler"]);
            result = true;
            return config;
        }
    }
}
