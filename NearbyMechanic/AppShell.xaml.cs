using NearbyMechanic.ViewModels;
using NearbyMechanic.Views;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace NearbyMechanic
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            //Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
            Routing.RegisterRoute(nameof(SignupPage), typeof(SignupPage));
            Routing.RegisterRoute(nameof(UsersListPage), typeof(UsersListPage));
            Routing.RegisterRoute(nameof(SupplierPage), typeof(SupplierPage));
        }

    }
}
