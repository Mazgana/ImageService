
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Modal.Event
{
    /*
     * this class holds arguments that will be passed when a close event occurs in program.
     */
    public class DirectoryCloseEventArgs : EventArgs
    {
        public string DirectoryPath { get; set; }       //the directory to be closed

        public string Message { get; set; }             // The Message That goes to the logger

        public DirectoryCloseEventArgs(string dirPath, string message)
        {
            DirectoryPath = dirPath;                    // Setting the Directory Name
            Message = message;                          // Storing the String
        }
    }
}
