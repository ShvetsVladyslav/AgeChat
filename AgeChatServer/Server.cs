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
        List<User> users;
        List<Client> connectedClients;
        public void Run()
        {
            var server = new Server();

            var listeningSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            listeningSocket.Bind(new IPEndPoint(IPAddress.Any, port: 8080));
            listeningSocket.Listen(0);
            connectedClients = new List<Client>();

            //Getting list of all existed users
            FillUserList();
            //

            while (true)
            {
                var clientSocket = listeningSocket.Accept();
                Console.WriteLine("not started yet");
                var nextThread = new Thread(new ThreadStart(() =>
                {
                    connectedClients.Add(new Client(clientSocket));
                    Console.WriteLine("A client connected.");
                    Console.WriteLine("Client's IpAddress is :" + IPAddress.Parse(((IPEndPoint)clientSocket.RemoteEndPoint).Address.ToString()) + ", connected on port number " + ((IPEndPoint)clientSocket.LocalEndPoint).Port.ToString());

                    HandShake(connectedClients[connectedClients.Count - 1].GetClientSocket());

                    while (true)
                    {
                        try
                        {
                            Request(connectedClients[connectedClients.Count - 1]);
                        }
                        catch (Exception e) { break; }
                    }
                }));
                nextThread.Start();
            }
        }
        public void Login(Client client)
        {
            Console.WriteLine("login started");
            string login = ReceiveMessage(client);
            string pass = ReceiveMessage(client);

            Console.WriteLine(login + " " + pass);

            DataBase db = new DataBase();
            MySqlCommand command = new MySqlCommand("SELECT * FROM `users` WHERE login = '" + login + "' AND passwordHash = SHA1('" + pass + "')", db.GetConnection());
            db.OpenConnection();
            MySqlDataReader reader;
            reader = command.ExecuteReader();
            reader.Read();
            if (reader.HasRows)
            {
                client.ConnectUser(new User());
                client.SetClientID(Int32.Parse(reader[0].ToString()));
                client.SetUsername(reader[3].ToString());
                //setting online status to user
                GoOnline(client);
                //
                Console.WriteLine("client logged in");
            }
            else
            {
                Console.WriteLine("no client found!");
            }
            db.CloseConnection();
        }

        public void MessageToGlobalChat(string msg, Client sender)
        {
            throw new NotImplementedException();
        }

        public void PersonalMessage(string msg, Client sender, Client receiver)
        {
            throw new NotImplementedException();
        }

        public void Registration(string log, string pass, string username)
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

        public void Request(Client client)
        {
            var receivedString = ReceiveMessage(client);
            IPAddress ip = IPAddress.Parse(((IPEndPoint)client.GetClientSocket().RemoteEndPoint).Address.ToString());

            if (connectedClients.Contains(client))
            {
                Console.WriteLine($"Client {ip}: {receivedString}");
            }
            
            if(receivedString == "login")
            {
                Login(client);
            }
            else
            {
                Console.WriteLine("Unknown command!");
            }
        }

        public void SendGlobalMessageList(Client client)
        {
            throw new NotImplementedException();
        }

        public void SendMessageList(Client receiver, Client sender)
        {
            throw new NotImplementedException();
        }

        public void HandShake(Socket clientSocket)
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
        private string ReceiveMessage(Client client)
        {
            IPAddress ip = IPAddress.Parse(((IPEndPoint)client.GetClientSocket().RemoteEndPoint).Address.ToString());
            var frameParser = new FrameParser();
            while (true)
            {
                var receivedData = new byte[1000000];
                string receivedString;
                client.GetClientSocket().Receive(receivedData);
                if ((receivedData[0] & (byte)Opcode.CloseConnection) == (byte)Opcode.CloseConnection)
                {
                    // Close connection request.
                    Console.WriteLine("Client with ip: " + ip + " disconnected!");
                    client.GetClientSocket().Close();
                    //setting offline status to user
                    GoOffline(client);
                    //
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
        private void FillUserList()
        {
            users = new List<User>();
            DataBase db = new DataBase();
            MySqlCommand command = new MySqlCommand("SELECT * FROM `users`", db.GetConnection());
            MySqlDataReader MyDataReader;
            db.OpenConnection();

            MyDataReader = command.ExecuteReader();
            while (MyDataReader.Read())
            {
                users.Add(new User());
                users[users.Count - 1].id = MyDataReader.GetInt32(0);
                users[users.Count - 1].username = MyDataReader.GetString(3);
            }

            db.CloseConnection();
            Console.WriteLine($"UserList initialized, there are {users.Count} registered users");
        }
        private void GoOnline(Client client)
        {
            for (int i = 0; i < users.Count; i++)
            {
                if (client.GetID() == users[i].id && client.GetUsername() == users[i].username)
                {
                    if (!users[i].IsOnline())
                    {
                        users[i].Online();
                        Console.WriteLine($"User {users[i].username} is online");
                    }
                }
            }
        }
        private void GoOffline(Client client)
        {
            if (client.GetUser() != null)
            {
                for (int i = 0; i < users.Count; i++)
                {
                    if (client.GetID() == users[i].id && client.GetUsername() == users[i].username)
                    {
                        if (users[i].IsOnline())
                        {
                            users[i].Offline();
                            Console.WriteLine($"User {users[i].username} is offline");
                        }
                    }
                }
            }
        }
    }
}