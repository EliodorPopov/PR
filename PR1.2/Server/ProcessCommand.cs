using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;

namespace ConsoleApp1
{
    static class ProcessCommand
    {
        private static DbContext db_context = new DbContext();
        public static byte[] Execute(string command)
        {
            Console.WriteLine("Executing command: " + command);
            string[] args = command.Split(' ');
            if(args[0] == "selectcolumn")
            {
                List<string> res = db_context.GetColumn(args[1]);
                string response = JArray.FromObject(res).ToString();
                Console.WriteLine("sending response: " + response);

                byte[] data = new byte[response.Length];
                Array.Copy(Encoding.ASCII.GetBytes(response), data, args.Length);
                return data;
            }

            return null;
        }
    }
}
