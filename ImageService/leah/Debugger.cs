using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageService.Communication;

namespace leah
{
    class Debugger
    {
        public Debugger()
        {
            Console.WriteLine("start\n");
            TcpClientChannel client = new TcpClientChannel();
            Console.WriteLine("end");
        }
    }
}
