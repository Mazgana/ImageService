using ImageService.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Commands
{
    /*
     * interface for commands that run in program, with execute function for running command.
     */
    public interface ICommand
    {
        string Execute(string[] args, out bool result);          // The Function That will Execute The Command
    }
}
