using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AgeChatServer
{
    class Program
    {
        static void Main(string[] args)
        {
            /*var server = new Server();

            var listeningSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            listeningSocket.Bind(new IPEndPoint(IPAddress.Any, port: 8080));
            listeningSocket.Listen(0);
            var connectedClients = new List<Client>();

            while (true)
            {
                var clientSocket = listeningSocket.Accept();
                connectedClients.Add(new Client(clientSocket));
                Console.WriteLine("A client connected.");
                Console.WriteLine("Client's IpAddress is :" + IPAddress.Parse(((IPEndPoint)clientSocket.RemoteEndPoint).Address.ToString()) + ", connected on port number " + ((IPEndPoint)clientSocket.LocalEndPoint).Port.ToString());
                Console.WriteLine("not started yet");
                var nextThread = new Thread(new ThreadStart(() =>
                {
                    Console.WriteLine("started");
                    server.run(connectedClients);
                }));
                nextThread.Start();
            }*/
            var server = new Server();
            server.run();
        }
    }
}
