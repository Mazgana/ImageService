using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageService.Modal;
using ImageService.Controller.Handlers;
using System.Configuration;

namespace ImageService.Commands
{
    class CloseHandlerCommand : ICommand
    {

        public CloseHandlerCommand() { }

        /// <summary>
        /// closing handlerby erasing it from config
        /// </summary>
        /// <param name="args"> containing hanlder to be closed</param>
        /// <param name="result"> return whether hanlder was found or erased</param>
        /// <returns> name of handler that was erased</returns>
        public string Execute(string[] args, out bool result)
        {
            result = false;
            //reading current handlers from config
            System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            string curHandlers = config.AppSettings.Settings["Handler"].Value;
            string newHandlers;
            string handler = args[0];
            //checking if the given handler is in config and cremove if so.
            if (curHandlers.Contains(handler))
            {
                result = true;
                newHandlers = curHandlers.Remove(curHandlers.IndexOf(handler), handler.Length);
            } else
            {
                newHandlers = curHandlers;
            }
            //update and save config
            config.AppSettings.Settings["Handler"].Value = newHandlers;
            config.Save(ConfigurationSaveMode.Modified);

            return handler;
        }
    }
}
