using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Client
{
    class ClientSocket
    {
        private Socket clientSocket;
        private byte[] buffer;
        private const int BUFF_SIZE = 4096;
        private string ip;
        private int port;

        public ClientSocket()
        {
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public void Connect(string ipAddres, int port)
        {
            ip = ipAddres;
            this.port = port;
            clientSocket.BeginConnect(new IPEndPoint(IPAddress.Parse(ipAddres), port), ConnectCallback, null);
        }

        private void Reconnect()
        {
            clientSocket.Close();
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            clientSocket.BeginConnect(new IPEndPoint(IPAddress.Parse(ip), port), ConnectCallback, null);
        }

        public void Send(byte[] data)
        {
            try
            {
                clientSocket.Send(data);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Could not Send data, {e.Message}");
            }
        }

        private void ConnectCallback(IAsyncResult result)
        {
            if (clientSocket.Connected)
            {
                try
                { 
                    Console.WriteLine("Connection estabilished");
                    buffer = new byte[BUFF_SIZE];
                    clientSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, ReceiveCallback, null);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Can't estabilish a connection, {e.Message}");
                }
            }
            else
            {
                Console.WriteLine("Client Socket could not connect, trying to reconnect in 5 seconds");
                Thread.Sleep(5000);
                this.Reconnect();
            }
        }

        private void ReceiveCallback(IAsyncResult result)
        {
            try
            {
                int buffLength = clientSocket.EndReceive(result);
                byte[] packet = new byte[buffLength];
                Array.Copy(buffer, packet, packet.Length);
                string res = Encoding.Default.GetString(packet);
                Console.WriteLine(res);

                buffer = new byte[BUFF_SIZE];
                clientSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, ReceiveCallback, null);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Can't receive data from server {e.Message}");
                Console.WriteLine("Trying to recconect in 5 seconds");
                Thread.Sleep(5000);
                this.Reconnect();
            }
        }
    }
}
