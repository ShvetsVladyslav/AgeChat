using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading;
using WebSocket4Net;
using Xamarin.Forms;

namespace AgeChatClient
{
    class Chats
    {
        WebSocket ws;
        List<string> messages;
        ObservableCollection<User> users;
        public Chats(WebSocket ws)
        {
            messages = new List<string>();
            users = new ObservableCollection<User>();
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
            for (int i = 0; i < messages.Count; i++)
            {
                users.Add(new User(messages[i]));
            }
            messages.Clear();
            ws.Send("showOn");
            Thread.Sleep(100);
            for (int i = 0; i < users.Count; i++)
            {
                for (int j = 0; j < messages.Count; j++)
                {
                    if (users[i].username == messages[j])
                    {
                        users[i].color = Color.LightGreen;
                        messages.RemoveAt(j);
                        break;
                    }
                }
            }
            User tmp;
            for (int i = 1; i < users.Count; i++)
            {
                if (users[i].color == Color.LightGreen)
                {
                    for (int j = 0; j < i; j++)
                    {
                        if (users[j].color == Color.LightGray)
                        {
                            tmp = users[j];
                            users[j] = users[i];
                            users[i] = tmp;
                        }
                    }
                }
            }
        }
        
        public ObservableCollection<User> userCollection 
        {
            get
            {
                return users;
            }
        }
    }

    class User
    {
        public User(string username)
        {
            this.username = username;
            color = Color.LightGray;
        }
        public string username{get; set;}
        public Color color{get; set;}
    }
}
