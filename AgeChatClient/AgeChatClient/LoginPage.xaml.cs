using System;
using System.Collections.Generic;
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
    public partial class LoginPage : ContentPage
    {
        WebSocket ws;
        List<string> messages;
        Page mainPage;
        public LoginPage(WebSocket ws, Page p)
        {
            InitializeComponent();
            mainPage = p;
            messages = new List<string>();
            this.ws = ws;
            ws.MessageReceived += new EventHandler<MessageReceivedEventArgs>(ReceivedMessage);
        }

        protected override void OnAppearing()
        {
            logo.Source = ImageSource.FromResource("AgeChatClient.Images.Logo.png");
            logo.Aspect = Aspect.AspectFill;

            if (ws.State != WebSocketState.Open)
            {
                DisplayAlert("Error", "Connection error!", "Ok").ContinueWith(t =>
                {
                    Environment.Exit(0);
                });
            }
        }

        private void ReceivedMessage(object sender, MessageReceivedEventArgs e)
        {
            messages.Add(e.Message);
        }

        private async void ButtonLogIn_ClickedAsync(object sender, EventArgs e)
        {
            if (ws != null && ws.State == WebSocketState.Open)
            {
                if (login.Text != null && password.Text != null)
                {
                    if (login.Text.Length != 0 && password.Text.Length != 0)
                    {
                        messages.Clear();
                        ws.Send("login");
                        Thread.Sleep(5);
                        ws.Send(login.Text);
                        Thread.Sleep(5);
                        ws.Send(password.Text);

                        int count = 0;
                        while (messages.Count == 0)
                        {
                            if (count == 500)
                            {
                                await DisplayAlert("Error", "Server failed to respond!", "Ok");
                            }
                            Thread.Sleep(10);
                            count++;
                        }

                        if (messages[0] == "user logged in")
                        {
                            messages.Clear();
                            ws.Send("getUsername");
                            while (messages.Count == 0)
                            {
                                Thread.Sleep(10);
                            }
                            Application.Current.Properties["username"] = messages[0];
                            Application.Current.Properties["isLoggedIn"] = "true";

                            await Navigation.PopModalAsync(true);
                        }
                        else
                        {
                            await DisplayAlert("Error", "Wrong login or password!", "Ok");
                        }
                    }
                    else
                    {
                        await DisplayAlert("", "Input correct data!", "Ok");
                    }
                }
                else
                {
                    await DisplayAlert("", "Input something!", "Ok");
                }
            }
        }

        private async void ButtonCreateNew_ClickedAsync(object sender, EventArgs e)
        {
            
        }
    }
}