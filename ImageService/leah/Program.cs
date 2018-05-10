using ImageService.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace leah
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("main\n");
            TcpClientChannel client = new TcpClientChannel();
            Console.WriteLine("end");
        }
    }
}
