using NearbyMechanic.Models;
using NearbyMechanic.Views;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using Firebase.Auth;
using NearbyMechanic.Assets;
using NearbyMechanic.Services;
using Rg.Plugins.Popup.Services;
using TourismAppV2.Dialogs.CustomDialogs;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using User = NearbyMechanic.Models.User;

namespace NearbyMechanic.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private FirebaseDatabase firebase;

        public Command LoginCommand { get; }
        public Command SignUpCommand { get;}
        public string Phone { get; set; }
        public string Password { get; set; }
        private int count = 0;

        public LoginViewModel()
        {
            firebase = new FirebaseDatabase();
            LoginCommand = new Command(OnLoginClicked);
            SignUpCommand = new Command(async () => await Shell.Current.GoToAsync($"{nameof(SignupPage)}"));
        }

        private async void OnLoginClicked(object obj)
        {
            if (count < 4)
            {
                await PopupNavigation.Instance.PushAsync(new LoadingDialog("Validating..."));
                if (await LoginMechanic())
                {
                    await Shell.Current.GoToAsync($"//{nameof(MainPage)}");
                }
                else
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(Phone) && !string.IsNullOrEmpty(Password))
                        {

                            var firebase = new FirebaseDatabase();
                            var result = await firebase.GetUserData(Phone);
                            if (result == null)
                            {
                                throw new TypeUnloadedException();
                            }

                            FirebaseAuthProvider authProvider = new FirebaseAuthProvider(new FirebaseConfig(FirebaseAssets.WebAPIKey));
                            var auth = await authProvider.SignInWithEmailAndPasswordAsync(result.Email, Password);
                            if (!string.IsNullOrEmpty(auth.RefreshToken))
                            {
                                var users = await DataStore.GetUsersAsync();
                                DataStore.SetCurrentUser(users.Find(ag => ag.Phone == Phone));
                                await Shell.Current.GoToAsync($"//{nameof(MainPage)}");
                            }

                        }
                    }
                    catch (FirebaseAuthException ex)
                    {
                        string err = null;
                        switch (ex.Reason)
                        {
                            case AuthErrorReason.MissingPassword:
                                err = "Please enter password";
                                break;
                            case AuthErrorReason.MissingEmail:
                                err = "Please enter email address";
                                break;
                            case AuthErrorReason.TooManyAttemptsTryLater:
                                err = "Too many login attempts";
                                break;
                            default:
                                err = "User with these credentials doesn't exists";
                                break;
                        }
                        await Shell.Current.DisplayAlert("Denied", err, "OK");
                    }
                    catch (Exception)
                    {
                        await Shell.Current.DisplayAlert("Denied", "User with these credentials doesn't exists", "OK");
                    }
                }
            }
            else
            {
                await Shell.Current.DisplayAlert("Denied", "You have exceeded your three time trial. Please try again later", "OK");
                
            }
            count++;
            await PopupNavigation.Instance.PopAllAsync();
        }

        private  async Task<bool> LoginMechanic()
        {
            try
            {
                dynamic raw = new
                {
                    Phone = Phone,
                    Password = Password
                };
                var content = new StringContent(JsonConvert.SerializeObject(raw), Encoding.UTF8, "application/json");
                var result = await HttpRequests._client.PostAsync(HttpRequests.BaseUri + "/auth.php?authenticate=true", content);
                result.EnsureSuccessStatusCode();
                if (result.IsSuccessStatusCode)
                {
                    string response = await result.Content.ReadAsStringAsync();
                    List<User> mechanic = JsonConvert.DeserializeObject<List<User>>(response);
                    if (mechanic != null && mechanic.Count > 0)
                    {
                        mechanic[0].PhotoUrl = FirebaseAssets.DefaultPhoto;
                        AddMechanicToDb(mechanic[0]);
                        var muser = await firebase.GetUserData(mechanic[0].Phone);
                        DataStore.SetCurrentUser(muser);
                        return true;
                    }
                }

                return false;
            }
            catch(Exception ex)
            {
                await Shell.Current.DisplayAlert("Denied", ex.Message, "OK");
                return false;
            }
        }

        private async void AddMechanicToDb(User data)
        {
            
            if(await firebase.GetUserData(data.Phone) == null)
            {
                await firebase.CreateNewUser(data);
            }
        }
    }
}
