using System;
using System.Text;

namespace Client
{
    class Message
    {
        public static byte[] CreateMessage(string msg)
        {
            byte[] packet = new byte[msg.Length+2];
            byte[] packetLength = BitConverter.GetBytes((ushort)msg.Length);
            Array.Copy(packetLength, packet, 2);
            Array.Copy(Encoding.ASCII.GetBytes(msg), 0, packet, 2, msg.Length);

            return packet;
        }
    }
}
