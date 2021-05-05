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
        public MainPage()
        {
            InitializeComponent();
            messages = new List<string>();
            Application.Current.Properties["isLoggedIn"] = "false";

            ws = new WebSocket("ws://192.168.0.109:8080/");
            ws.EnableAutoSendPing = false;
            ws.MessageReceived += new EventHandler<MessageReceivedEventArgs>(ReceivedMessage);

            ws.Open();

            LoginPage loginPage = new LoginPage(ws, this);
            Navigation.PushModalAsync(loginPage, false);
        }

        protected override void OnAppearing()
        {
        }

        private void ReceivedMessage(object sender, MessageReceivedEventArgs e)
        {
            messages.Add(e.Message);
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            if (messages.Count != 0)
            {
                lbl1.Text = messages[0].ToString();
                messages.RemoveAt(0);
            }
            else
            {
                lbl1.Text = "";
            }
        }

        private void btnDisconnect_click(object sender, EventArgs e)
        {
            if (Application.Current.Properties["isLoggedIn"].ToString() == "true")
            {
                DisplayAlert(Application.Current.Properties["username"].ToString(), "good", "ok");
                Title = Application.Current.Properties["username"].ToString();
            }
            /*ws.Close();
            Environment.Exit(0);*/
        }

        private void btnSend_click(object sender, EventArgs e)
        {
            if (ws != null && ws.State == WebSocketState.Open && text.Text.Length != 0)
            {
                ws.Send(text.Text);
                Thread.Sleep(10);
                if (messages.Count != 0)
                {
                    lbl1.Text = messages[0].ToString();
                    messages.RemoveAt(0);
                }
                else
                {
                    lbl1.Text = "";
                }
            }
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }
    }
}
