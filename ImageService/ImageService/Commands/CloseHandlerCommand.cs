using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageService.Modal;
using ImageService.Controller.Handlers;

namespace ImageService.Commands
{
    class CloseHandlerCommand : ICommand
    {
        private DirectoyHandler handler;

        public CloseHandlerCommand(DirectoyHandler h)
        {
            handler = h;            // Storing the Modal
        }

        // change!!!!!
        public string Execute(string[] args, out bool result)
        {
            result = true;
            return "ok";
        }
    }
}
