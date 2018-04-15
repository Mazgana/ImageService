using ImageService.Infrastructure;
using ImageService.Modal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Commands
{
    /*
     * class for command to be executed when a new file appears in watched directory
     * that needs to be moved to new directory.
     */
    public class NewFileCommand : ICommand
    {
        private IImageServiceModal m_modal;

        //constructor. storing the modal
        public NewFileCommand(IImageServiceModal modal)
        {
            m_modal = modal;
        }

        //// Will Return the New Path if result = true, and will return the error message otherwise
        public string Execute(string[] args, out bool result)
        {
            return m_modal.AddFile(args[0], out result);
        }
    }
}
