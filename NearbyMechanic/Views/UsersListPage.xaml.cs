using NearbyMechanic.Models;
using NearbyMechanic.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;
using NearbyMechanic.Services;
using Rg.Plugins.Popup.Services;
using TourismAppV2.Dialogs.CustomDialogs;

namespace NearbyMechanic.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UsersListPage : ContentPage
    {
        private FirebaseDatabase firebase;
        private string phone;
        public UsersListPage()
        {
            InitializeComponent();
            firebase = new FirebaseDatabase();
            phone = firebase.DataStore.GetCurrentUser().Result.Phone;


        }

        private async void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            User item = e.Item as User;
            string[] actions = { "See on map", "Call", "Send Text SMS", "Send Request", "Cancel request" };
            var result = await Shell.Current.DisplayActionSheet("Action", "Cancel", null, actions);
            if(result == "See on map")
            {
                try
                {
                    Location location = await Geolocation.GetLocationAsync(new GeolocationRequest
                    { DesiredAccuracy = GeolocationAccuracy.High, Timeout = TimeSpan.FromSeconds(30) });
                    await Map.OpenAsync(location.Latitude, location.Longitude);
                }
                catch
                {
                    await Shell.Current.DisplayAlert("Location error", "Please enable location and try again", "OK");
                }
            }
            else if(result == "Call")
            {
                PhoneDialer.Open(item.Phone);
            }
            else if(result == "Send Text SMS")
            {
                await Sms.ComposeAsync(new SmsMessage
                {
                    Body = "Compose request message",
                    Recipients = new List<string> { item.Phone }
                });
            }
            else if(result == "Send Request")
            {
                try
                {
                    if (item.Progress == "Progress" || item.Progress == "Responding")
                    {
                        await DisplayAlert("Ongoing", "You already requested a pending service from this mechanic", "OK");
                    }
                    else
                    {
                        await PopupNavigation.Instance.PushAsync(new LoadingDialog("Sending request..."));
                        Location location = await Geolocation.GetLocationAsync(new GeolocationRequest
                        { DesiredAccuracy = GeolocationAccuracy.High, Timeout = TimeSpan.FromSeconds(30) });

                        var driver = await firebase.DataStore.GetCurrentUser();
                        driver.Latitude = location.Latitude;
                        driver.Longitude = location.Longitude;

                        await firebase.UpdateUser(driver);

                        var results = await firebase.GetAllJobs();
                        if (results.Any(ag => ag.MechanicPhone == item.Phone && ag.ClientPhone == phone))
                        {
                            var newStatus = results.Find(ag => ag.MechanicPhone == item.Phone && ag.ClientPhone == phone);
                            newStatus.Status = false;
                            newStatus.Progress = "Pending";

                            await firebase.UpdateJob(newStatus);
                        }
                        else
                        {
                            Jobs job = new Jobs
                            {
                                Status = false,
                                MechanicPhone = item.Phone,
                                DateOfRequest = DateTime.Now,
                                ClientPhone = phone,
                                Progress = "Pending"
                            };
                            await firebase.SubmitRequest(job);
                        }
                        
                    }
                    
                }
                catch
                {
                    await Shell.Current.DisplayAlert("Location error", "Please enable location and try again", "OK");
                }
                finally
                {
                    await PopupNavigation.Instance.PopAllAsync();
                }
            }
            else if (result == "Cancel request")
            {
                await PopupNavigation.Instance.PushAsync(new LoadingDialog("Cancelling request..."));
                var results = await firebase.GetAllJobs();
                var newStatus = results.Find(ag => ag.MechanicPhone == item.Phone && ag.ClientPhone == phone);
                newStatus.Status = true;
                newStatus.Progress = "Completed";

                await firebase.UpdateJob(newStatus);
                await PopupNavigation.Instance.PopAllAsync();
            }
            else
            {
                return;
            }
        }

        private async void Job_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            User item = e.Item as User;
            var response = await Shell.Current.DisplayActionSheet("Action", "OK", null, new string[] { "See on map", "Respond", "See details", "Call", "Send SMS", "Cancel request" });

            if (response == "Respond")
            {
                await PopupNavigation.Instance.PushAsync(new LoadingDialog("Confirming request..."));
                var results = await firebase.GetAllJobs(phone);
                var newStatus = results.Find(ag => ag.ClientPhone == item.Phone);
                newStatus.Status = true;
                newStatus.Progress = "Responding";

                await firebase.UpdateJob(newStatus);
                await PopupNavigation.Instance.PopAllAsync();
            }
            else if (response == "Cancel request")
            {
                await PopupNavigation.Instance.PushAsync(new LoadingDialog("Confirming request..."));
                var results = await firebase.GetAllJobs(phone);
                var newStatus = results.Find(ag => ag.ClientPhone == item.Phone);
                newStatus.Status = true;
                newStatus.Progress = "Completed";

                await firebase.UpdateJob(newStatus);
                await PopupNavigation.Instance.PopAllAsync();
            }
            else if (response == "Call")
            {
                PhoneDialer.Open(item.Phone);
            }
            else if(response == "See on map")
            {
                try
                {
                    await Map.OpenAsync(item.Latitude, item.Longitude);
                }
                catch
                {
                    await Shell.Current.DisplayAlert("Location error", "Please enable location and try again", "OK");
                }
            }
            else if (response == "See details")
            {
                await Shell.Current.DisplayAlert("Driver details", $"Driver: {item.Fullname} \r\n Phone: {item.Phone} \r\n Email: {item.Email} \r\n Car make: {item.CarMake}", "OK");
            }
            else if(response == "Send SMS")
            {
                await Sms.ComposeAsync();
            }
            else
            {
                return;
            }
        }
    }
}