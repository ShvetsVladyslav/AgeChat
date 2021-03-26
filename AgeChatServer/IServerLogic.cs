using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace AgeChatServer
{
    interface IServerLogic
    {
        void Run();
        void HandShake(Socket clientSocket);
        void Request(Client client);
        void Login(Client client);
        void Logout(Client client);
        void Registration(Client client);
        void MessageToGlobalChat(string msg, Client sender);
        void PersonalMessage(string msg, Client sender);
        void SendGlobalMessageList(Client client);
        void SendMessageList(Client receiver, Client sender);
    }
}
