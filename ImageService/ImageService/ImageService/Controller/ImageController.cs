using ImageService.Commands;
using ImageService.Infrastructure;
using ImageService.Infrastructure.Enums;
using ImageService.Modal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ImageService.Controller
{
    /*
     * the controller class gets a command id and string and executes the specific matching command.
     */
    public class ImageController : IImageController
    {
        private IImageServiceModal m_modal;                      // The Modal Object.
        private Dictionary<int, ICommand> commands;              //Dictionairy holding all commands with their ID.

        public ImageController(IImageServiceModal modal)
        {
            m_modal = modal;                    // Storing the Modal Of The System
            commands = new Dictionary<int, ICommand>() //Adding existing command to Dictionary
            {
                { 1 ,new NewFileCommand(modal)},
            };
        }

        //this function gets a fcommand ID, checks if it is in dictionary, and executed the matching command.
        public string ExecuteCommand(int commandID, string[] args, out bool resultSuccesful)
        {
            //check if in dictionairy else result=fail
            if (commands.ContainsKey(commandID))
            {
                return commands[commandID].Execute(args, out resultSuccesful);//run command
            } else
            {
                resultSuccesful = false;
                return "no such command";
            }
        }
    }
}
