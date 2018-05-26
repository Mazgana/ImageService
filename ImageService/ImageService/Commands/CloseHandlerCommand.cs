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

        public string Execute(string[] args, out bool result)
        {
            result = false;
            System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            string curHandlers = config.AppSettings.Settings["Handler"].Value;
            string newHandlers;
            string handler = args[0];

            if (curHandlers.Contains(handler))
            {
                result = true;
                newHandlers = curHandlers.Remove(curHandlers.IndexOf(handler), handler.Length);
            } else
            {
                newHandlers = curHandlers;
            }
            config.AppSettings.Settings["Handler"].Value = newHandlers;
            config.Save(ConfigurationSaveMode.Modified);

            return handler;
        }
    }
}
