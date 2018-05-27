using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Infrastructure.Enums
{
    /// <summary>
    /// enum for commands in solution
    /// </summary>
    public enum CommandEnum : int
    {
        NewFileCommand,
        GetConfigCommand,
        LogCommand,
        CloseCommand
    }
}
