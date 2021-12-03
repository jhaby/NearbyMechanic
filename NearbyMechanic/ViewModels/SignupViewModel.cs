using System;
using System.Collections.Generic;
using System.Text;
using Firebase.Auth;
using NearbyMechanic.Assets;
using NearbyMechanic.Services;
using Rg.Plugins.Popup.Services;
using TourismAppV2.Dialogs.CustomDialogs;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace NearbyMechanic.ViewModels
{
    public class SignupViewModel
    {
        public string Fullname { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Password { get; set; }
        public string CarModel { get; set; }
        public string CarMake { get; set; }
        public string PlateNumber { get; set; }
        public Command SignupCommand { get; }
        private FirebaseDatabase firebase = new FirebaseDatabase();
        public SignupViewModel()
        {
            SignupCommand = new Command(Signup);
        }

        private void Signup()
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                try
                {
                    await PopupNavigation.Instance.PushAsync(new LoadingDialog("Submitting data..."));
                    Models.User userData = new Models.User
                    {
                        Fullname = Fullname,
                        Email = Email,
                        Phone = Phone,
                        Address = Address,
                        CarMake = CarMake,
                        CarModel = CarModel,
                        CarPlateNo = PlateNumber,
                        IsDriver = true,
                        IsMechanic = false,
                        PhotoUrl = FirebaseAssets.DefaultPhoto
                    };
                    if (Phone.Length == 10 && Phone.IndexOf("07") == 1)
                    {
                        FirebaseAuthProvider authProvider = new FirebaseAuthProvider(new FirebaseConfig(FirebaseAssets.WebAPIKey));
                        var auth = await authProvider.CreateUserWithEmailAndPasswordAsync(Email, Password);

                        await firebase.CreateNewUser(userData);

                        await Shell.Current.GoToAsync("..");
                    }
                    else
                    {
                        await PopupNavigation.Instance.PushAsync(new LoadingDialog("Invalid phone number"));
                    }

                }
                catch (FirebaseAuthException ex)
                {
                    string err = null;
                    switch (ex.Reason)
                    {
                        case AuthErrorReason.Undefined:
                            err = "Failed to create user";
                            break;
                        case AuthErrorReason.InvalidEmailAddress:
                            err = "Invalid email address";
                            break;
                        case AuthErrorReason.MissingPassword:
                            err = "Please enter password";
                            break;
                        case AuthErrorReason.WeakPassword:
                            err = "Password too weak, should be atleast 8 characters";
                            break;
                        case AuthErrorReason.EmailExists:
                            err = "Emails already exists";
                            break;
                        case AuthErrorReason.MissingEmail:
                            err = "Please enter email";
                            break;
                        case AuthErrorReason.DuplicateCredentialUse:
                            err = "User already exists";
                            break;
                        default:
                            break;
                    }
                    await Shell.Current.DisplayAlert("Failed", err, "OK");
                }
                catch(Exception)
                {
                    await Shell.Current.DisplayAlert("error", "Failed to create user account", "OK");
                }
                finally
                {
                    await PopupNavigation.Instance.PopAllAsync();

                }
            });
        }
    }
}
