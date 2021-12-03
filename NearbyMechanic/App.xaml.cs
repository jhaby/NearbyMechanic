using NearbyMechanic.Services;
using NearbyMechanic.Views;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NearbyMechanic
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();

            DependencyService.Register<DataStore>();
            MainPage = new AppShell();
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
