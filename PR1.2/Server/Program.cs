using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.IO;
namespace ConsoleApp1
{
    class Program
    {
        private static ServerSocket socket = new ServerSocket();
        static void Main(string[] args)
        {
            socket.Bind("127.0.0.1", 9000);
            socket.Listen();
            socket.Accept();
            Console.WriteLine("Server started and listening");
            Console.ReadLine();
        }
    }
}
