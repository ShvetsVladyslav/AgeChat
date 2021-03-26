using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace AgeChatServer
{
    class Client
    {
        private Socket clientSocket;
        private User logginedUser;
        public Client(Socket sok) 
        {
            this.clientSocket = sok;
        }
        public Socket GetClientSocket()
        {
            return clientSocket;
        }
        public void ConnectUser(User user)
        {
            this.logginedUser = user;
        }
        public void DisconnectUser()
        {
            this.logginedUser = null;
        }
        public User GetUser()
        {
            return logginedUser;
        }
        public int GetID()
        {
            return this.logginedUser.id;
        }
        public string GetUsername()
        {
            return this.logginedUser.username;
        }
        public void SetClientID(int id)
        {
            this.logginedUser.id = id;
        }
        public void SetUsername(string Username)
        {
            this.logginedUser.username = Username;
        }
    }
}
