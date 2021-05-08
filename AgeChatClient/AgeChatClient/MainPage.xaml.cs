﻿using System;
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
        Chats chatsPage;
        bool isTimerEnabled = true;
        public MainPage(WebSocket ws)
        {
            InitializeComponent();
            messages = new List<string>();
            this.ws = ws;
            ws.MessageReceived += ReceivedMessage;
            chatsPage = new Chats(ws);
            this.BindingContext = chatsPage;
        }

        protected override void OnAppearing()
        {
            labelUsername.Text = $"Logged in as {App.Current.Properties["username"]}.";
            Device.StartTimer(TimeSpan.FromSeconds(10), TimerTick);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            isTimerEnabled = false;
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
        private bool TimerTick()
        {
            chatsPage.UpdateOnlineUsers();
            return isTimerEnabled;
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