using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Threading;
using WebSocket4Net;
using Xamarin.Forms;

namespace AgeChatClient
{
    class Chats : INotifyPropertyChanged
    {
        WebSocket ws;
        List<string> messages;
        ObservableCollection<User> users;

        public event PropertyChangedEventHandler PropertyChanged;

        public Chats(WebSocket ws)
        {
            messages = new List<string>();
            users = new ObservableCollection<User>();
            this.ws = ws;
            ws.MessageReceived += ReceivedMessage;
            users.Add(new User("Global Chat"));
            users[0].color = Color.LightGreen;
            GetUsers();
        }

        private void ReceivedMessage(object sender, MessageReceivedEventArgs e)
        {
            messages.Add(e.Message);
        }

        private void GetUsers()
        {
            ws.Send("showAll");
            Thread.Sleep(50);
            for (int i = 0; i < messages.Count; i++)
            {
                users.Add(new User(messages[i]));
            }
            UpdateOnlineUsers();
        }

        public void UpdateOnlineUsers()
        {
            messages.Clear();
            ws.Send("showOn");
            Thread.Sleep(50);
            for (int i = 1; i < users.Count; i++)
            {
                bool isStillOnline = false;
                for (int j = 0; j < messages.Count; j++)
                {
                    if (users[i].username == messages[j])
                    {
                        if(users[i].color == Color.LightGray)
                        {
                            User tmp = new User(users[i].username);
                            tmp.color = Color.LightGreen;
                            users.RemoveAt(i);
                            users.Insert(i, tmp);
                            isStillOnline = true;
                            messages.RemoveAt(j);
                            break;
                        }
                        else if (users[i].color == Color.LightGreen)
                        {
                            isStillOnline = true;
                            messages.RemoveAt(j);
                            break;
                        }
                    }
                }
                if (!isStillOnline)
                {
                    User tmp = new User(users[i].username);
                    tmp.color = Color.LightGray;
                    users.RemoveAt(i);
                    users.Insert(i, tmp);
                }
            }
            SortUsers();
        }
        
        public ObservableCollection<User> userCollection 
        {
            get
            {
                return users;
            }
            set 
            {
                userCollection = value;
                OnPropertyChanged("userCollection");
            }
        }

        private void SortUsers()
        {
            User tmp;
            for (int i = 2; i < users.Count; i++)
            {
                if (users[i].color == Color.LightGreen)
                {
                    for (int j = 1; j < i; j++)
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

        private void OnPropertyChanged(string name = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
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
