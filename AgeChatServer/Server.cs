using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using MySql.Data.MySqlClient;

namespace AgeChatServer
{
    class Server : IServerLogic
    {
        public void run()
        {
            var server = new Server();

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
                    handShake(connectedClients[connectedClients.Count - 1].clientSocket);
                    while (true)
                    {
                        Console.WriteLine("started");
                        request(connectedClients[connectedClients.Count - 1], connectedClients);
                    }
                }));
                nextThread.Start();
            }
        }
        public void login(Client client, List<Client> connectedClients)
        {
            Console.WriteLine("login started");
            string login = receiveMessage(client, connectedClients);
            string pass = receiveMessage(client, connectedClients);

            Console.WriteLine(login + " " + pass);

            client.connectUser(new User());
            DataBase db = new DataBase();
            MySqlCommand command = new MySqlCommand("SELECT * FROM `users` WHERE login = '" + login + "' AND passwordHash = SHA1('" + pass + "')", db.getConnection());
            db.openConnection();
            MySqlDataReader reader;
            reader = command.ExecuteReader();
            reader.Read();
            /*while (reader.Read())
            {
                if (login == (reader["login"].ToString()) && SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(pass)) == Encoding.UTF8.GetBytes(reader["passwordHash"].ToString()))
                {
                    client.setClientID(Int32.Parse(reader["ClientID"].ToString()));
                    client.setUsername(reader["UserName"].ToString());
                    Console.WriteLine("client logged in");
                    break;
                }
            }*/
            if (reader.HasRows)
            {
                client.setClientID(Int32.Parse(reader[0].ToString()));
                client.setUsername(reader[3].ToString());
                Console.WriteLine("client logged in");
            }
            else
            {
                Console.WriteLine("no client found!");
            }
            db.closeConnection();
        }

        public void messageToGlobalChat(string msg, Client sender, List<Client> connectedClients)
        {
            throw new NotImplementedException();
        }

        public void personalMessage(string msg, Client sender, Client receiver, List<Client> connectedClients)
        {
            throw new NotImplementedException();
        }

        public void registration(string log, string pass, string username, List<Client> connectedClients)
        {
            /*DataBase db = new DataBase();
            MySqlCommand command = new MySqlCommand(db.getConnection());
            db.openConnection();
            try
            {
                // Команда Insert.
                string sql = "Insert into User (Login, Password, Username) "
                                                 + " values (@Login, @Password, @Username) ";

                SqlCommand cmd = command.CreateCommand();
                cmd.CommandText = sql;

                // Создать объект Parameter.
                cmd.Parameters.Add("@Login", SqlDbType.string).Value = log

                // Добавить параметр @highSalary (Написать короче).
                cmd.Parameters.Add("@Password", SqlDbType.string).Value = pass;

                // Добавить параметр @lowSalary (Написать короче).
                cmd.Parameters.Add("@Username", SqlDbType.string).Value = username;

                // Выполнить Command (Используется для delete, insert, update).
                int rowCount = cmd.ExecuteNonQuery();

                Console.WriteLine("Row Count affected = " + rowCount);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e);
                Console.WriteLine(e.StackTrace);
            }
            finally
            {
                connection.Close();
                connection.Dispose();
                connection = null;
            }

            Console.Read();*/

        }

        public void request(Client client, List<Client> connectedClients)
        {
            var receivedString = receiveMessage(client, connectedClients);
            Console.WriteLine($"Client: {receivedString}");
            if(receivedString == "login")
            {
                login(client, connectedClients);
            }
            else
            {
                Console.WriteLine("Unknown command!");
            }
        }

        public void sendGlobalMessageList(Client client, List<Client> connectedClients)
        {
            throw new NotImplementedException();
        }

        public void sendMessageList(Client receiver, Client sender, List<Client> connectedClients)
        {
            throw new NotImplementedException();
        }

        public void handShake(Socket clientSocket)
        {
            var receivedData = new byte[1000000];
            var receivedDataLength = clientSocket.Receive(receivedData);
            Console.WriteLine("handshake started");
            var requestString = Encoding.UTF8.GetString(receivedData, 0, receivedDataLength);

            if (new Regex("^GET").IsMatch(requestString))
            {
                const string eol = "\r\n";

                var receivedWebSocketKey = new Regex("Sec-WebSocket-Key: (.*)").Match(requestString).Groups[1].Value.Trim();
                var keyHash = SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(receivedWebSocketKey + "258EAFA5-E914-47DA-95CA-C5AB0DC85B11"));

                var response = "HTTP/1.1 101 Switching Protocols" + eol;
                response += "Connection: Upgrade" + eol;
                response += "Upgrade: websocket" + eol;
                response += "Sec-WebSocket-Accept: " + Convert.ToBase64String(keyHash) + eol;
                response += eol;

                clientSocket.Send(Encoding.UTF8.GetBytes(response));
            }
        }
        private string receiveMessage(Client client, List<Client> connectedClients)
        {
            var frameParser = new FrameParser();
            while (true)
            {
                var receivedData = new byte[1000000];
                string receivedString;
                client.clientSocket.Receive(receivedData);
                if ((receivedData[0] & (byte)Opcode.CloseConnection) == (byte)Opcode.CloseConnection)
                {
                    // Close connection request.
                    Console.WriteLine("Client disconnected.");
                    client.clientSocket.Close();
                    connectedClients.Remove(client);
                    return "";
                }
                else
                {
                    receivedString = frameParser.ParsePayloadFromFrame(receivedData);
                    return receivedString;
                }
            }
        }
    }
}