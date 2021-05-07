using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using Xamarin.Forms;
using WebSocket4Net;
using System.Threading.Tasks;

namespace AgeChatClient
{
    public partial class MainPage : ContentPage
    {
        WebSocket ws;
        List<string> messages;
        public MainPage(WebSocket ws)
        {
            InitializeComponent();
            messages = new List<string>();
            this.ws = ws;
            ws.MessageReceived += ReceivedMessage;
            this.BindingContext = new Chats(ws);
        }

        protected override void OnAppearing()
        {
            labelUsername.Text = $"Logged in as {App.Current.Properties["username"]}.";
        }
        private void ReceivedMessage(object sender, MessageReceivedEventArgs e)
        {
            messages.Add(e.Message);
        }
        protected override bool OnBackButtonPressed()
        {
            return true;
        }
        private void ButtonExit_Clicked(object sender, EventArgs e)
        {
            ws.Close();
            Environment.Exit(0);
        }
        private void ButtonLogout_Clicked(object sender, EventArgs e)
        {
            App.Current.Properties["username"] = "";
            ws.Send("logout");
            Application.Current.MainPage = new LoginPage(ws);
        }
    }
}

/*if (messages.Count != 0)
{
    lbl1.Text = messages[0].ToString();
    messages.RemoveAt(0);
}
else
{
    lbl1.Text = "";
}*/