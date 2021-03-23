using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeChatServer
{
    interface IServerLogic
    {
        void run();
        void request(string method);
        void login(string log, string pass);
        void registration(string log, string pass, string username);
        void messageToGlobalChat(string msg, Client sender);
        void personalMessage(string msg, Client sender, Client receiver);
        void sendGlobalMessageList(Client client);
        void sendMessageList(Client receiver, Client sender);
    }
}
