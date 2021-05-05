using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AgeChatClient
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            NavigationPage navigation = new NavigationPage(new MainPage())
            {
                //BarBackgroundColor = Color.LimeGreen,
                BarBackgroundColor = Color.Transparent,
                BarTextColor = Color.Black
            };

            MainPage = navigation;
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
