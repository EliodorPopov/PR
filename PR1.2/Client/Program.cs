using System;

namespace Client
{
    class Program
    {
        private static ClientSocket socket = new ClientSocket();
        static void Main(string[] args)
        {
            socket.Connect("127.0.0.1", 9000);
            Console.WriteLine("/q - exit");

            string command = "";
            while (true)
            {
                command = Console.ReadLine();
                if (command.ToLower() == "/q")
                {
                    return;
                }
                else if (String.IsNullOrWhiteSpace(command))
                {
                    continue;
                }

                var message = Message.CreateMessage(command);

                socket.Send(message);
            }
        }
    }
}
