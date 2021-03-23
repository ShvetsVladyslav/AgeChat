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
        void run();
        void handShake(Socket clientSocket);
        void request(Client client, List<Client> connectedClients);
        void login(Client client, List<Client> connectedClients);
        void registration(string log, string pass, string username, List<Client> connectedClients);
        void messageToGlobalChat(string msg, Client sender, List<Client> connectedClients);
        void personalMessage(string msg, Client sender, Client receiver, List<Client> connectedClients);
        void sendGlobalMessageList(Client client, List<Client> connectedClients);
        void sendMessageList(Client receiver, Client sender, List<Client> connectedClients);
    }
}
