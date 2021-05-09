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
            Task.Run(() =>
            {
                Thread.Sleep(50);
                if (messages.Count != 0)
                    Dispatcher.BeginInvokeOnMainThread(() => messageList.ScrollTo(messages[messages.Count - 1], ScrollToPosition.End, true));
            });
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