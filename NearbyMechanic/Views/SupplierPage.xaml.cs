using NearbyMechanic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NearbyMechanic.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SupplierPage : ContentPage
    {
        public SupplierPage()
        {
            InitializeComponent();
        }

        private async void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            User item = e.Item as User;
            string[] actions = { "Call", "Send Text SMS" };
            var result = await Shell.Current.DisplayActionSheet("Action", "Cancel", null, actions);
            if (result == "Call")
            {
                PhoneDialer.Open(item.Phone);
            }
            else if (result == "Send Text SMS")
            {
                await Sms.ComposeAsync(new SmsMessage
                {
                    Body = "Compose request message",
                    Recipients = new List<string> { item.Phone }
                });
            }
            else
            {
                return;
            }
        }
    }
}