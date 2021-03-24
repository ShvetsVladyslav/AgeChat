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
        void Registration(string log, string pass, string username);
        void MessageToGlobalChat(string msg, Client sender);
        void PersonalMessage(string msg, Client sender, Client receiver);
        void SendGlobalMessageList(Client client);
        void SendMessageList(Client receiver, Client sender);
    }
}
