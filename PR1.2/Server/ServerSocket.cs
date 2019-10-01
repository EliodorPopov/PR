using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class ServerSocket
    {
        private const int DEFAULT_CONNECTIONS_NR = 100;
        private Socket serverSocket;

        public ServerSocket()
        {
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public void Bind(string ipString, int port)
        {
            serverSocket.Bind(new IPEndPoint(IPAddress.Parse(ipString), port));
        }

        public void Listen(int nrOfConnections = DEFAULT_CONNECTIONS_NR)
        {
            serverSocket.Listen(nrOfConnections);
        }

        public void Accept()
        {
            serverSocket.BeginAccept(AcceptedCallback, null);
        }

        private void AcceptedCallback(IAsyncResult result)
        {
            try
            {
                ConnectionInfo connection = new ConnectionInfo();

                connection.socket = serverSocket.EndAccept(result);
                connection.data = new byte[ConnectionInfo.BUFF_SIZE];
                connection.socket.BeginReceive(connection.data, 0, connection.data.Length, SocketFlags.None, ReceiveCallback, connection);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Can't accept, {e.Message}");
            }
            finally
            {
                Accept();
            }

        }

        private void ReceiveCallback(IAsyncResult result)
        {
            ConnectionInfo connection = result.AsyncState as ConnectionInfo;

            try
            {
                Socket clientSocket = connection.socket;
                SocketError response;
                int buffSize = clientSocket.EndReceive(result, out response);

                if (response == SocketError.Success)
                {
                    byte[] packet = new byte[buffSize];
                    Array.Copy(connection.data, packet, packet.Length);

                    PacketHandler.Handle(packet, clientSocket);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Can't receive data from Client, {e.Message}");
            }
            finally
            {
                try
                {
                    connection.socket.BeginReceive(connection.data, 0, connection.data.Length, SocketFlags.None, ReceiveCallback, connection);
                }
                catch(Exception e)
                {
                    Console.WriteLine($"{e.Message}");
                    connection.socket.Close();
                }
            }
        }
    }
}
