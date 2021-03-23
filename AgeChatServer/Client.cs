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
        public Socket clientSocket;
        private User logginedUser;
        public Client(Socket sok) 
        {
            this.clientSocket = sok;
        }
        public void connectUser(User user)
        {
            this.logginedUser = user;
        }
        public int getID()
        {
            return this.logginedUser.clientID;
        }
        public string getUsername()
        {
            return this.logginedUser.username;
        }
        public void setClientID(int id)
        {
            this.logginedUser.clientID = id;
        }
        public void setUsername(string Username)
        {
            this.logginedUser.username = Username;
        }
    }
}
