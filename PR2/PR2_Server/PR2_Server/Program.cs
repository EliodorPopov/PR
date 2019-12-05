using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PR2_Server
{
    class Program
    {
        private const int listenPort = 11001; // To send a datagram using UDP, you must know the network address of the network device hosting the service you need and the UDP port number that the service uses to communicate
        public static Socket serverSocket; // this will be the global socket
        public static int counter = 0; // this is basically the ID / Key of the message sent
        public static Dictionary<int, string> data = new Dictionary<int, string>(); // Here i will globally store the data, in an ideal world, this would be a database
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to the UDP server!");
            
            // Initialise socket
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            Console.WriteLine("Server initialised");

            // Broadcast to local ip
            IPAddress broadcast = IPAddress.Parse(GetLocalIPAddress());

            // Menu controller
            while (true)
            {
                Console.WriteLine("Select option:");
                Console.WriteLine("1. Send Messages");
                Console.WriteLine("2. See sent messages");
                Console.WriteLine("3. Exit");
                Console.Write("Your option: ");

                string option = Console.ReadLine();

                try
                {
                    short optionNumber = short.Parse(option);
                    switch (optionNumber)
                    {
                        case 1:
                            Console.Clear();
                            SendMessages(broadcast);
                            Console.Clear();
                            break;
                        case 2:
                            Console.Clear();
                            PrintAllSentMessages();
                            Console.Clear();
                            break;
                        case 3:
                            Console.Clear();
                            Console.WriteLine("Goodbye!");
                            Thread.Sleep(2000);
                            return;
                        default:
                            throw new Exception();
                    }
                }
                catch
                {
                    Console.WriteLine("Invalid option");
                }
            }
        }

        public static void SendMessages(IPAddress broadcast) // This method controlls the sending of the messages and the user interaction
        {
            Console.WriteLine("Enter your message to send and press enter. If you want to exit, write 'exit'");
            IPEndPoint ep = new IPEndPoint(broadcast, listenPort);

            // Get message
            string message = Console.ReadLine();
            while (message != "exit")
            {
                data.Add(counter, message); // Add message to dictionary in order to be able to control what was sent and what the client didn't receive
                SendMessage(ep, message, counter);
                counter++;
                message = Console.ReadLine();
            }
        }

        public static void SendMessage(IPEndPoint ep, string decryptedMessage, int id)
        {
            /*
             * This method adds the decrypted message's length to the first to bytes of the buffer
             * To the second 2 bytes of the buffer i add the id in order to be able to identify which messages were sent
             * In order not to lock the thread, i use the method SendToAsync and then i asynchronously wait for the response from the client
             * to know that it was received successfully
             * 
             * 
             * 
            */
            string message = Encryption.Encrypt(decryptedMessage);
            byte[] sendbuf = new byte[message.Length + 4];
            byte[] packetLength = BitConverter.GetBytes(message.Length);
            byte[] counterBytes = BitConverter.GetBytes(id);
            Array.Copy(packetLength, sendbuf, 2);
            Array.Copy(counterBytes, 0, sendbuf, 2, 2);
            Array.Copy(Encoding.ASCII.GetBytes(message), 0, sendbuf, 4, message.Length);
            var args = new SocketAsyncEventArgs();
            args.SetBuffer(sendbuf, 0, sendbuf.Length);
            args.RemoteEndPoint = ep;
            serverSocket.SendToAsync(args);
            Console.WriteLine("Message sent to the broadcast address");
            ReceiveResponseAsync(ep);
        }

        public static async void ReceiveResponseAsync(IPEndPoint ep) // This method waits for the response asynchrounosly and send the data to HandleResponse method
        {
            await Task.Run(() =>
            {
                var args = new SocketAsyncEventArgs();
                int buffLength = serverSocket.ReceiveBufferSize;
                byte[] packet = new byte[buffLength];
                args.SetBuffer(packet, 0, buffLength);
                args.Completed += async (_, arguments) => await HandleResponse(arguments, ep);
                args.Buffer[0] = 0;
                serverSocket.ReceiveAsync(args);
            });
        }

        private static async Task HandleResponse(SocketAsyncEventArgs args, IPEndPoint ep) // The client will either send a success method or the id of the data it doesn't have
        {
            await Task.Run(() =>
            {
                string res = Encoding.Default.GetString(args.Buffer.Where(x => x != 0).ToArray());
                Console.WriteLine(res);
                if (res != "success")
                {
                    try
                    {
                        int id = Int32.Parse(res);
                        if (data[id] != null)
                        {
                            SendMessage(ep, data[id], id);
                        }
                        else
                        {
                            throw new Exception();
                        }
                    }
                    catch
                    {
                        Console.WriteLine($"An error has occured, response {res} could not be processed");
                    }
                }
            });
        }

        public static void PrintAllSentMessages() // obviously
        {
            if (data.Count == 0)
            {
                Console.Write("No data");
                Console.ReadLine();
            }
            else
            {
                foreach (KeyValuePair<int, string> entry in data)
                {
                    Console.WriteLine($"{entry.Key}. {entry.Value}");
                }
                Console.ReadLine();
            }
        }

        public static string GetLocalIPAddress() // it's clear in the name
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }
    }
}
