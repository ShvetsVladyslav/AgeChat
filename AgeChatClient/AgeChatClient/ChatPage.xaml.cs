using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebSocket4Net;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AgeChatClient
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChatPage : ContentPage
    {
        WebSocket ws;
        bool isScrolling = false;
        public ObservableCollection<Msg> messages { get; set; }
        public ChatPage(WebSocket ws, string username)
        {
            InitializeComponent();
            Title = username;
            this.ws = ws;
            ws.MessageReceived += new EventHandler<MessageReceivedEventArgs>(ReceivedMessage);
            messages = new ObservableCollection<Msg>();
            this.BindingContext = this;
            messageList.Margin = 10;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (Title != "Global Chat")
            {
                ws.Send("pchistory");
                Thread.Sleep(5);
                ws.Send(Title);
            }
            else
            {
                ws.Send("gchistory");
            }
        }

        private void ReceivedMessage(object sender, MessageReceivedEventArgs e)
        {
            if (Title != "Global Chat")
            {
                if (e.Message.StartsWith(Title) || e.Message.StartsWith(App.Current.Properties["username"].ToString()))
                {
                    Dispatcher.BeginInvokeOnMainThread(() => messages.Add(new Msg(e.Message)));
                } 
            }
            else
            {
                Dispatcher.BeginInvokeOnMainThread(() => messages.Add(new Msg(e.Message)));
            }
            Task.Run(() =>
            {
                isScrolling = true;
                Thread.Sleep(50);
                if (messages.Count != 0 && !isScrolling)
                {
                    Dispatcher.BeginInvokeOnMainThread(() => messageList.ScrollTo(messages[messages.Count - 1], ScrollToPosition.End, true));
                }
                isScrolling = false;
            });
        }

        private void ButtonSend_Clicked(object sender, EventArgs e)
        {
            if (messageText.Text != null)
            {
                if (messageText.Text.Length != 0)
                {
                    string messageToSend = messageText.Text.Trim();

                    if (Title != "Global Chat")
                    {
                        messages.Add(new Msg(App.Current.Properties["username"].ToString() + ": " + messageToSend));
                        messageText.Text = "";
                        ws.Send("pm");
                        Thread.Sleep(5);
                        ws.Send(messageToSend);
                        Thread.Sleep(5);
                        ws.Send(Title);
                    }
                    else
                    {
                        messages.Add(new Msg(App.Current.Properties["username"].ToString() + ": " + messageToSend));
                        messageText.Text = "";
                        ws.Send("gm");
                        Thread.Sleep(5);
                        ws.Send(messageToSend);
                    }
                    messageList.ScrollTo(messages[messages.Count - 1], ScrollToPosition.End, true);
                }
            }
        }
    }
    public class Msg
    {
        public Msg(string msg)
        {
            message = msg;
        }
        public string message { get; set; }
    }
}