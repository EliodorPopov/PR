using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class ConnectionInfo
    {
        public byte[] data = new byte[1024];
        public Socket socket;
        public const int BUFF_SIZE = 1024;
    }
}
