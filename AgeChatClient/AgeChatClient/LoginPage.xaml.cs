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
        bool isLoggedIn = false;
        public LoginPage(WebSocket ws)
        {
            InitializeComponent();
            messages = new List<string>();

            this.ws = ws;
            if (ws.State != WebSocketState.Open)
            {
                ws.EnableAutoSendPing = false;
                ws.Open();
            }
            ws.MessageReceived += new EventHandler<MessageReceivedEventArgs>(ReceivedMessage);

            login.Completed += (sender, e) =>
            {
                password.Focus();
            };
            password.Completed += (sender, e) =>
            {
                if (repeatPassword.IsVisible)
                {
                    repeatPassword.Focus();
                }
                else
                {
                    ButtonLogIn_ClickedAsync(sender, e);
                }
            };
            repeatPassword.Completed += (sender, e) =>
            {
                usernameTextBox.Focus();
            };
            usernameTextBox.Completed += (sender, e) =>
            {
                CreateAccountButton_Clicked(sender, e);
            };
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
            if (ws.State == WebSocketState.Open && !isLoggedIn)
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
                        await Task.Run(() =>
                        {
                            while (messages.Count == 0)
                            {
                                if (count == 500)
                                {
                                    DisplayAlert("Error", "Server failed to respond!", "Ok");
                                }
                                Thread.Sleep(10);
                                count++;
                            }
                        });

                        if (messages[0] == "user logged in")
                        {
                            isLoggedIn = true;
                            await Task.Run(() =>
                            {
                                messages.Clear();
                                ws.Send("getUsername");
                                while (messages.Count == 0)
                                {
                                    Thread.Sleep(10);
                                }
                                Application.Current.Properties["username"] = messages[0];
                            });

                            Application.Current.MainPage = new NavigationPage(new MainPage(ws) { Title = "Chats" })
                            {
                                BarBackgroundColor = Color.FromHex("#139e2f"),
                                BarTextColor = Color.White,
                            };
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

        private void ButtonCreateNew_Clicked(object sender, EventArgs e)
        {
            SwitchLoginPage();
        }

        private void ButtonHaveAccount_Clicked(object sender, EventArgs e)
        {
            SwitchLoginPage();
        }

        private async void CreateAccountButton_Clicked(object sender, EventArgs e)
        {
            if (ws.State == WebSocketState.Open && !isLoggedIn)
            {
                if (login.Text != null && password.Text != null && repeatPassword.Text != null && usernameTextBox.Text != null)
                {
                    if (login.Text.Length != 0 && password.Text.Length != 0 && repeatPassword.Text.Length != 0 && usernameTextBox.Text.Length != 0)
                    {
                        if (password.Text == repeatPassword.Text)
                        {
                            messages.Clear();
                            ws.Send("registration");
                            Thread.Sleep(5);
                            ws.Send(login.Text);
                            Thread.Sleep(5);
                            ws.Send(password.Text);
                            Thread.Sleep(5);
                            ws.Send(usernameTextBox.Text);

                            int count = 0;
                            await Task.Run(() =>
                            {
                                while (messages.Count == 0)
                                {
                                    if (count == 500)
                                    {
                                        DisplayAlert("Error", "Server failed to respond!", "Ok");
                                    }
                                    Thread.Sleep(10);
                                    count++;
                                }
                            });

                            if (messages[0] == $"User {usernameTextBox.Text} registered!")
                            {
                                await DisplayAlert("Success", "Congratulations! Your account was created!\nYou can sign in using it now!", "Let's go!");
                                SwitchLoginPage();
                            }
                            else
                            {
                                await DisplayAlert("Error", messages[0], "Ok");
                            }
                        }
                        else
                        {
                            await DisplayAlert("Error", "password fields do not match!", "Ok");
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

        private void SwitchLoginPage()
        {
            if (loginButton.IsVisible)
            {
                pageTitleLabel.Text = "Registration";
            }
            else
            {
                pageTitleLabel.Text = "Sign in";
            }
            repeatPassword.IsVisible = !repeatPassword.IsVisible;
            usernameTextBox.IsVisible = !usernameTextBox.IsVisible;
            createAccountButton.IsVisible = !createAccountButton.IsVisible;
            haveAccountButton.IsVisible = !haveAccountButton.IsVisible;
            loginButton.IsVisible = !loginButton.IsVisible;
            createNewAccountButton.IsVisible = !createNewAccountButton.IsVisible;
        }
    }
}