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
                var nextThread = new Thread(new ThreadStart(() =>
                {
                    connectedClients.Add(new Client(clientSocket));
                    Console.WriteLine("A client connected.");
                    Console.WriteLine("Client's IpAddress is :" + IPAddress.Parse(((IPEndPoint)clientSocket.RemoteEndPoint).Address.ToString()) + ", connected on port number " + ((IPEndPoint)clientSocket.LocalEndPoint).Port.ToString());

                    HandShake(connectedClients[connectedClients.Count - 1].GetClientSocket());

                    Client activeClient = new Client(clientSocket);

                    while (true)
                    {
                        try
                        {
                            Request(activeClient);
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
            if (client.GetUser() == null)
            {
                string login = ReceiveMessage(client);
                string pass = ReceiveMessage(client);

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
                    SendMessage("user logged in", client.GetClientSocket());
                }
                else
                {
                    Console.WriteLine("no client found!");
                    SendMessage("user not found", client.GetClientSocket());
                }
                db.CloseConnection();
            }
            else
            {
                Console.WriteLine("user already logged in");
                SendMessage("You're already logged in!", client.GetClientSocket());
            }
        }
        public void Logout(Client client)
        {
            if (client.GetUser() != null)
            {
                GoOffline(client);
                client.DisconnectUser();
                Console.WriteLine("client logged out");
            }
        }

        public void MessageToGlobalChat(Client sender)
        {
            Console.WriteLine("Global chat message");
            if (sender.GetUser() != null)
            {
                string msg = ReceiveMessage(sender);
                DataBase db = new DataBase();
                db.OpenConnection();
                try
                {
                    DateTime sendTime = new DateTime();
                    sendTime = DateTime.Now;
                    string sql = "INSERT INTO `globalmessages` (senderId, messageText, datetime) values (@sender, @msg, @datetime)";
                    MySqlCommand command = new MySqlCommand(sql, db.GetConnection());

                    command.Parameters.Add("@msg", MySqlDbType.VarChar).Value = msg;
                    command.Parameters.Add("@sender", MySqlDbType.String).Value = sender.GetID();
                    command.Parameters.Add("@datetime", MySqlDbType.DateTime).Value = sendTime;
                    int rowCount = command.ExecuteNonQuery();
                    Console.WriteLine($"Message saved!\nRows affected = " + rowCount);

                    for (int i = 0; i < connectedClients.Count; i++)
                    {
                        if (connectedClients[i].GetUser() != null)
                        {
                            if (connectedClients[i].GetID() != sender.GetID())
                            {
                                msg = sender.GetUsername() + ": " + msg;
                                //SendMessage(sender.GetUsername(), connectedClients[i].GetClientSocket());
                                SendMessage(msg, connectedClients[i].GetClientSocket());
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error: " + e);
                    //SendMessage($"Error: message is abnormal!", sender.GetClientSocket());
                }
                db.CloseConnection();
            }
            else
            {
                SendMessage("You have to log in first!", sender.GetClientSocket());
            }
        }

        public void PersonalMessage(Client sender)
        {
            Console.WriteLine("Personal message start");
            if (sender.GetUser() != null)
            {
                string msg = ReceiveMessage(sender);

                string receiver = ReceiveMessage(sender);

                for (int i = 0; i < users.Count; i++)
                {
                    if (receiver == users[i].username)
                    {
                        DataBase db = new DataBase();
                        db.OpenConnection();
                        try
                        {
                            DateTime sendTime = new DateTime();
                            sendTime = DateTime.Now;
                            string sql = "INSERT INTO `messages` (senderId, receiverId, messageText, datetime) values (@sender, @receiver, @msg, @datetime)";
                            MySqlCommand command = new MySqlCommand(sql, db.GetConnection());

                            command.Parameters.Add("@msg", MySqlDbType.VarChar).Value = msg;
                            command.Parameters.Add("@sender", MySqlDbType.Int32).Value = sender.GetID();
                            command.Parameters.Add("@receiver", MySqlDbType.Int32).Value = users[i].id;
                            command.Parameters.Add("@datetime", MySqlDbType.DateTime).Value = sendTime;
                            int rowCount = command.ExecuteNonQuery();
                            Console.WriteLine($"Message saved!\nRows affected = " + rowCount);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Error: " + e);
                            SendMessage($"Error: message is abnormal!", sender.GetClientSocket());
                        }
                        db.CloseConnection();
                        if (users[i].IsOnline())
                        {
                            for (int j = 0; j < connectedClients.Count; j++)
                            {
                                if (connectedClients[j].GetUser() != null)
                                {
                                    if (users[i].username == connectedClients[j].GetUsername())
                                    {
                                        msg = sender.GetUsername() + ": " + msg;
                                        SendMessage(msg, connectedClients[j].GetClientSocket());
                                        Console.WriteLine("message sent to " + receiver);
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine("Receiver offline");
                            break;
                        }
                    }
                }
            }
            else
            {
                SendMessage("You have to log in first!", sender.GetClientSocket());
            }
        }

        public void Registration(Client client)
        {
            Console.WriteLine("registration started");
            if (client.GetUser() == null)
            {
                string login = ReceiveMessage(client);
                string pass = ReceiveMessage(client);
                string username = ReceiveMessage(client);

                pass = string.Concat(SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(pass)).Select(b => b.ToString("x2")));

                DataBase db = new DataBase();
                db.OpenConnection();
                try
                {
                    string sql = "INSERT INTO `users` (login, passwordHash, username) VALUES (@Login, @Password, @Username) ";
                    MySqlCommand command = new MySqlCommand(sql, db.GetConnection());

                    command.Parameters.Add("@Login", MySqlDbType.VarChar).Value = login;
                    command.Parameters.Add("@Password", MySqlDbType.String).Value = pass;
                    command.Parameters.Add("@Username", MySqlDbType.VarChar).Value = username;
                    int rowCount = command.ExecuteNonQuery();

                    Console.WriteLine($"User {username} registered!\nRows affected = " + rowCount);
                    SendMessage($"User {username} registered!", client.GetClientSocket());
                    FillUserList();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error: " + e);
                    if (e.ToString().Contains("Duplicate entry"))
                    {
                        if (e.ToString().Contains("for key 'login'"))
                        {
                            SendMessage("User with this login is already exist!", client.GetClientSocket());
                        }
                        else if (e.ToString().Contains("for key 'username'"))
                        {
                            SendMessage("User with this username is already exist!", client.GetClientSocket());
                        }
                    }
                    else
                    {
                        SendMessage("Error: registration failed!", client.GetClientSocket());
                    }
                }
                db.CloseConnection();
            }
            else
            {
                Console.WriteLine("user already logged in");
                SendMessage("You already have account!", client.GetClientSocket());
            }
        }

        public void Request(Client client)
        {
            for (int i = 0; i < connectedClients.Count; i++)
            {
                if (client.GetClientSocket() == connectedClients[i].GetClientSocket())
                {
                    client = connectedClients[i];
                }
            }

            var receivedString = ReceiveMessage(client);
            IPAddress ip = IPAddress.Parse(((IPEndPoint)client.GetClientSocket().RemoteEndPoint).Address.ToString());

            if (connectedClients.Contains(client) && receivedString != "")
            {
                Console.WriteLine($"Client {ip}: {receivedString}");
            }
            
            if(receivedString == "login")
            {
                Login(client);
            }
            else if (receivedString == "registration")
            {
                Registration(client);
            }
            else if (receivedString == "logout")
            {
                Logout(client);
            }
            else if (receivedString == "gm")
            {
                MessageToGlobalChat(client);
            }
            else if (receivedString == "pm")
            {
                PersonalMessage(client);
            }
            else if (receivedString == "gchistory")
            {
                SendGlobalMessageHistory(client);
            }
            else if (receivedString == "pchistory")
            {
                SendMessageHistory(client);
            }
            else if (receivedString == "showAll")
            {
                ShowAllUsers(client);
            }
            else if (receivedString == "showOn")
            {
                ShowOnlineUsers(client);
            }
            else if (receivedString == "getUsername")
            {
                SendUsername(client);
            }
            else if (receivedString == "")
            {
                Disconnect(client);
            }
            else
            {
                Console.WriteLine("Unknown command!");
                SendMessage("Unknown command!", client.GetClientSocket());
            }
        }

        public void SendGlobalMessageHistory(Client client)
        {
            Console.WriteLine("Sending global chat history");
            if (client.GetUser() != null)
            {
                DataBase db = new DataBase();
                MySqlCommand command = new MySqlCommand("SELECT * FROM `globalmessages`", db.GetConnection());
                MySqlDataReader myDataReader;
                db.OpenConnection();

                myDataReader = command.ExecuteReader();
                while (myDataReader.Read())
                {
                    string username = "";
                    for (int i = 0; i < users.Count; i++)
                    {
                        if (myDataReader.GetInt32(1) == users[i].id)
                        {
                            username = users[i].username;
                        }
                    }
                    string line = $"{username}: {myDataReader.GetString(2)}";
                    SendMessage(line, client.GetClientSocket());
                }

                db.CloseConnection();
            }
            else
            {
                SendMessage("You have to log in first!", client.GetClientSocket());
            }
        }
        public void SendMessageHistory(Client sender)
        {
            Console.WriteLine("Sending private chat history");
            if (sender.GetUser() != null)
            {
                string receiver = ReceiveMessage(sender);
                for (int i = 0; i < users.Count; i++)
                {
                    if (receiver == users[i].username)
                    {
                        int receiverId = users[i].id;
                        DataBase db = new DataBase();
                        MySqlCommand command = new MySqlCommand("SELECT * FROM messages WHERE receiverId = '" + receiverId + "' AND senderId = " + sender.GetID()
                            + " OR receiverId = '" + sender.GetID() + "' AND senderId = " + receiverId, db.GetConnection());
                        MySqlDataReader myDataReader;
                        db.OpenConnection();

                        myDataReader = command.ExecuteReader();
                        List<DBMessage> messageList = new List<DBMessage>();
                        while (myDataReader.Read())
                        {
                            string senderUsername = "";
                            for (int j = 0; j < users.Count; j++)
                            {
                                if (myDataReader.GetInt32(1) == users[j].id)
                                {
                                    senderUsername = users[j].username;
                                }
                            }
                            string line = $"{senderUsername}: {myDataReader.GetString(3)}";
                            messageList.Add(new DBMessage(line, myDataReader.GetDateTime(4)));
                        }
                        DBMessage tmp;
                        for (int j = 0; j < messageList.Count; j++)
                        {
                            for (int k = 1; k < messageList.Count; k++)
                            {
                                if (messageList[k].time < messageList[k - 1].time)
                                {
                                    tmp = messageList[k - 1];
                                    messageList[k - 1] = messageList[k];
                                    messageList[k] = tmp;
                                }
                            }
                        }
                        for (int j = 0; j < messageList.Count; j++)
                        {
                            SendMessage(messageList[j].message, sender.GetClientSocket());
                        }

                        db.CloseConnection();
                        break;
                    }
                }
            }
            else
            {
                SendMessage("You have to log in first!", sender.GetClientSocket());
            }
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
        public void ShowOnlineUsers(Client client)
        {
            if (client.GetUser() != null)
            {
                for (int i = 0; i < users.Count; i++)
                {
                    if (users[i].IsOnline() && users[i].id != client.GetID())
                    {
                        SendMessage(users[i].username, client.GetClientSocket());
                    }
                }
            }
            else
            {
                SendMessage("You have to log in first!", client.GetClientSocket());
            }
        }
        public void ShowAllUsers(Client client)
        {
            if (client.GetUser() != null)
            {
                for (int i = 0; i < users.Count; i++)
                {
                    if (users[i].id != client.GetID())
                    {
                        SendMessage(users[i].username, client.GetClientSocket());
                    }
                }
            }
            else
            {
                SendMessage("You have to log in first!", client.GetClientSocket());
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
                    Disconnect(client);
                    return "";
                }
                else
                {
                    receivedString = frameParser.ParsePayloadFromFrame(receivedData);
                    return receivedString;
                }
            }
        }
        private void SendMessage(string message, Socket receiver)
        {
            FrameParser frameParser = new FrameParser();
            var dataToSend = frameParser.CreateFrameFromString(message);
            receiver.Send(dataToSend);
        }
        private void SendUsername(Client client)
        {
            Console.WriteLine("Sending UserName");
            if (client.GetUser() != null)
            {
                SendMessage(client.GetUsername(), client.GetClientSocket());
            }
            else
            {
                SendMessage("You have to log in first!", client.GetClientSocket());
            }
        }
        private void FillUserList()
        {
            users = new List<User>();
            DataBase db = new DataBase();
            MySqlCommand command = new MySqlCommand("SELECT * FROM `users`", db.GetConnection());
            MySqlDataReader myDataReader;
            db.OpenConnection();

            myDataReader = command.ExecuteReader();
            while (myDataReader.Read())
            {
                users.Add(new User());
                users[users.Count - 1].id = myDataReader.GetInt32(0);
                users[users.Count - 1].username = myDataReader.GetString(3);
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
        private void Disconnect(Client client)
        {
            IPAddress ip = IPAddress.Parse(((IPEndPoint)client.GetClientSocket().RemoteEndPoint).Address.ToString());
            // Close connection request.
            Console.WriteLine("Client with ip: " + ip + " disconnected!");
            client.GetClientSocket().Close();
            //setting offline status to user
            GoOffline(client);
            //
            connectedClients.Remove(client);
            Thread.CurrentThread.Abort();
        }
    }
    
    public class DBMessage
    {
        public DBMessage(string message, DateTime time)
        {
            this.message = message;
            this.time = time;
        }
        public string message { get; set; }
        public DateTime time { get; set; }
    }

}