using NearbyMechanic.Models;
using NearbyMechanic.Views;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using Xamarin.Essentials;
using Rg.Plugins.Popup.Services;
using TourismAppV2.Dialogs.CustomDialogs;
using NearbyMechanic.Services;

namespace NearbyMechanic.ViewModels
{
    public class MainPageViewModel : BaseViewModel
    {
        private User userData;

        public string ListLabel { get; set; }
        public Command UsersListCommand { get; }
        public Command AddSkillCommand { get;}
        public Command ImageTappedCommand { get; }
        public Command LocationCommand { get;}
        public Command SetLocationCommand { get; }
        public Command ViewSuppliersCommand { get; }
        public User UserData
        {
            get => userData;
            set
            {
                userData = value;
                OnPropertyChanged("UserData");
            }
        }
        public MainPageViewModel()
        {
            UserData = new User();
            UsersListCommand = new Command(async () => await Shell.Current.GoToAsync($"{nameof(UsersListPage)}"));
            AddSkillCommand = new Command(AddSkill);
            ImageTappedCommand = new Command(ChangeProfileImage);
            SetLocationCommand = new Command(SetLocation);
            ViewSuppliersCommand = new Command(async () => await Shell.Current.GoToAsync($"{nameof(SupplierPage)}"));
            LoadData();
        }

        private async void LoadData()
        {
            UserData = await DataStore.GetCurrentUser();
            if (UserData.IsMechanic)
            {
                ListLabel = "See jobs";
            }
            else
            {
                ListLabel = "See nearby mechanics";
            }
        }
        private async void AddSkill()
        {
            string value = await Shell.Current.DisplayPromptAsync("Skill", "Add type of vehicle you can repair");
            if (!string.IsNullOrEmpty(value))
            {
                var user = await DataStore.GetCurrentUser();
                user.Skills.Add(value);
                UserData = user;
            }
        }
        private void ChangeProfileImage()
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                try
                {
                    await PopupNavigation.Instance.PushAsync(new LoadingDialog("Uploading photo..."));
                    var photo = await MediaPicker.PickPhotoAsync();
                    if (photo != null)
                    {
                        using (var stream = await photo.OpenReadAsync())
                        {
                            var firebaseStorage = new FBStorage();
                            string uri = UserData.Phone + DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss");
                            UserData.PhotoUrl = await firebaseStorage.SaveImageAsync(stream, "Profiles", uri);

                            var firebaseDb = new FirebaseDatabase();
                            await firebaseDb.UpdateUser(UserData);
                            LoadData();
                        }
                    }


                }
                catch (PermissionException)
                {
                    await Application.Current.MainPage.DisplayAlert("Permission", "App has no permission to access your gallery", "Cancel");
                }
                catch (Exception ex)
                {
                    await Application.Current.MainPage.DisplayAlert("Failed", ex.Message, "Cancel");
                }
                finally
                {
                    await PopupNavigation.Instance.PopAsync();
                }
            });
        }
        private void SetLocation()
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                try
                {
                    await PopupNavigation.Instance.PushAsync(new LoadingDialog("Capturing location..."));
                    Location location = await Geolocation.GetLocationAsync(new GeolocationRequest
                    {
                        DesiredAccuracy = GeolocationAccuracy.High,
                        Timeout = TimeSpan.FromSeconds(30)
                    });
                    User newUserData = new User();
                    newUserData = UserData;
                    newUserData.Latitude = location.Latitude;
                    newUserData.Longitude = location.Longitude;
                    var firebaseDb = new FirebaseDatabase();
                    await firebaseDb.UpdateUser(newUserData);
                }
                catch(FeatureNotEnabledException)
                {
                    await Shell.Current.DisplayAlert("Location error", "Please enable location and try again", "OK");
                }
                catch(Exception ex)
                {
                    await Shell.Current.DisplayAlert("Location error", ex.Message, "OK");
                }
                finally
                {
                    await PopupNavigation.Instance.PopAsync();
                }
            });
        }
    }
}
