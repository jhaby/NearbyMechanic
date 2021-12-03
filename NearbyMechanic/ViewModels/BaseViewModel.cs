using Firebase.Auth;
using NearbyMechanic.Assets;
using NearbyMechanic.Models;
using NearbyMechanic.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xamarin.Forms;
using User = NearbyMechanic.Models.User;

namespace NearbyMechanic.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public IDataStore<User> DataStore => DependencyService.Get<IDataStore<User>>();

        bool isBusy = false;
        public bool IsBusy
        {
            get { return isBusy; }
            set { SetProperty(ref isBusy, value); }
        }

        string title = string.Empty;
        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }

        public User CurrentUser { get; set; }
        protected bool SetProperty<T>(ref T backingStore, T value,
            [CallerMemberName] string propertyName = "",
            Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }
        public void SetCurrentUser()
        {
            CurrentUser = DataStore.GetCurrentUser().Result;
        }

        public async Task<string> GetFirebaseToken()
        {
            FirebaseAuthProvider authprovider = new FirebaseAuthProvider(new FirebaseConfig(FirebaseAssets.WebAPIKey));
            dynamic credentials = await DataStore.GetFirebaseCredentials();
            FirebaseAuthLink authrequest = await authprovider.SignInWithEmailAndPasswordAsync(credentials.Email, credentials.Password);

            return authrequest.FirebaseToken;
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
