using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using WebSocket4Net;

namespace AgeChatClient
{
    class Chats
    {
        WebSocket ws;
        List<string> messages;
        string[] users;
        public Chats(WebSocket ws)
        {
            messages = new List<string>();
            this.ws = ws;
            ws.MessageReceived += ReceivedMessage;
            GetUsers();
        }

        private void ReceivedMessage(object sender, MessageReceivedEventArgs e)
        {
            messages.Add(e.Message);
        }

        private void GetUsers()
        {
            ws.Send("showAll");
            Thread.Sleep(100);
            users = new string[messages.Count];
            for (int i = 0; i < messages.Count; i++)
            {
                users[i] = messages[i];
            }
            messages.Clear();
            ws.Send("showOn");
            Thread.Sleep(100);
        }
        
        public string[] userCollection 
        {
            get
            {
                return users;
            }
        }

    }
}
