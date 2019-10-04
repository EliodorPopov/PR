using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;

namespace PR1
{
    static class ProcessCommand
    {
        public static byte[] Execute(string command)
        {
            Console.WriteLine("Executing command: " + command);
            string[] args = command.Split(' ');
            string response = "Invalid command";
            try
            {
                if (args[0] == "selectcolumn")
                {
                    List<string> res = Helpers.GetColumn(Program.fetchedResult, args[1]);
                    response = JArray.FromObject(res).ToString();
                }
            }
            catch (Exception) { 
                response = "An  internal error occured.";
            }
            byte[] packet = new byte[response.Length + 2];
            byte[] packetLength = BitConverter.GetBytes((ushort)response.Length);
            Array.Copy(packetLength, packet, 2);
            Array.Copy(Encoding.ASCII.GetBytes(response), 0, packet, 2, response.Length);

            return packet;

        }
    }
}
