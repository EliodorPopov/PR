using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace PR2_Client
{
    class Program
    {
        private const int listenPort = 11001;
        public static Dictionary<int, string> dataDictionary = new Dictionary<int, string>();
        private static void StartListener()
        {

            // Initialise listener
            UdpClient listener = new UdpClient(listenPort);
            IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, listenPort);

            try
            {
                while (true)
                {
                    Console.WriteLine("Waiting for broadcast");

                    // Reading the buffer the same way i sent it from the server
                    byte[] bytes = listener.Receive(ref groupEP);
                    ushort packetLength = BitConverter.ToUInt16(bytes, 0);
                    ushort id = BitConverter.ToUInt16(bytes, 2);
                    byte[] data = new byte[packetLength];
                    Array.Copy(bytes, 4, data, 0, packetLength);


                    string message = Encryption.Decrypt(Encoding.Default.GetString(data));
                    Console.Write($"Received broadcast from {groupEP} :");
                    Console.WriteLine($" {message}");


                    int messageLength = bytes.Length - 4; // 4 beacause the first 4 bytes store message length and id

                    if (messageLength != packetLength) // if the length stored in the package and the actual length are different, ask the server again for the package
                    {
                        byte[] sendbuf = Encoding.ASCII.GetBytes(id.ToString());
                        listener.Send(sendbuf, sendbuf.Length, groupEP);
                    }
                    else
                    {
                        dataDictionary[id] = message; // force add or override the message to the dictionary
                        if (id != 0) // if this is not the first message, check if the previous message was received recurrsevly
                        {
                            try
                            {

                                var a = dataDictionary[id - 1];
                                byte[] sendbuf = Encoding.ASCII.GetBytes("success"); // send a success message if the previous one is stored
                                listener.Send(sendbuf, sendbuf.Length, groupEP);
                            }
                            catch
                            {
                                byte[] sendbuf = Encoding.ASCII.GetBytes((id - 1).ToString()); // ask for the previous package if there is nothing there
                                listener.Send(sendbuf, sendbuf.Length, groupEP);
                            }
                        }
                    }

                }
            }
            catch (SocketException e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                listener.Close();
            }
        }

        static void Main(string[] args)
        {
            StartListener();
        }
    }
}
