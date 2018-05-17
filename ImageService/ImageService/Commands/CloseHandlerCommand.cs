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
        //private DirectoyHandler handler;
        private IImageServiceModal m_modal;

        //       public CloseHandlerCommand(DirectoyHandler h)
        public CloseHandlerCommand(IImageServiceModal model)
        {
            this.m_modal = model;
        //    handler = h;            // Storing the Modal
        }

        public string Execute(string[] args, out bool result)
        {
            string sResult = "closed";
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
                sResult = "could not find handler in config";
                //    newHandlers = "C:\Users\user\Downloads; C: \Users\as\Pictures; C: \Users\as\Videos; C: \Users\user\Desktop\listen";
                newHandlers = curHandlers;
            }
            config.AppSettings.Settings["Handler"].Value = newHandlers;
            config.Save(ConfigurationSaveMode.Modified);

            return sResult;
        }
    }
}
