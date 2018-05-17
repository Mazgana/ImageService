using ImageService.Modal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Commands
{
    class LogCommand : ICommand
    {
        private IImageServiceModal m_modal;

        /// <summary>
        /// Constructor. saves modal as member.
        /// </summary>
        /// <param name="modal"> The modal that will make the command operations. </param>
        public LogCommand(IImageServiceModal modal)
        {
            m_modal = modal;            // Storing the Modal
        }

        /// <summary>
        /// executes command to send the log of the application.
        /// </summary>
        /// <param name="args"> string arguments that is empty. </param>
        /// <returns> The return value is all the app config file properties concat to one stirng, 
        /// and will return the error message. </returns>
        public string Execute(string[] args, out bool result)
        {
            result = true;
            return "log";
        }
    }
}
