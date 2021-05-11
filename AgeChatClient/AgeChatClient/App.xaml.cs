using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using WebSocket4Net;

namespace AgeChatClient
{
    public partial class App : Application
    {
        WebSocket ws;
        public App()
        {
            InitializeComponent();

            ws = new WebSocket("ws://192.168.0.109:8080/");

            MainPage = new LoginPage(ws);
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
            if (Device.RuntimePlatform == Device.UWP)
            {
                if (ws.State == WebSocketState.Open)
                {
                    ws.Close();
                }
            }
        }

        protected override void OnResume()
        {
        }
    }
}
