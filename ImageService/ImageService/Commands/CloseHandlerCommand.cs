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
        //private DirectoyHandler handler;
        private IImageServiceModal m_modal;

        //       public CloseHandlerCommand(DirectoyHandler h)
        public CloseHandlerCommand(IImageServiceModal model)
        {
            this.m_modal = model;
        //    handler = h;            // Storing the Modal
        }

        // change!!!!!
        public string Execute(string[] args, out bool result)
        {
            result = true;
            return "close";
        }
    }
}
