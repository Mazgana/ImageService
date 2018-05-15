using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Commands
{
    class GetConfigCommand : ICommand
    {
        public GetConfigCommand(StreamWriter sw)
        {
           // private String outputDir = ConfigurationManager.AppSettings["OutputDir"];
        }

        /// <summary>
        /// executes commandy.
        /// </summary>
        /// <param name="args"> string arguments . </param>
        /// <returns> The String Will Return </returns>
        public string Execute(string[] args, out bool result)
        {
            result = true;
            return "sent";
        }
    }
}

