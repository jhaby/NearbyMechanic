using NearbyMechanic.Models;
using NearbyMechanic.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Xamarin.Forms;

namespace NearbyMechanic.ViewModels
{
    public class UsersListViewModel : BaseViewModel
    {
        private ObservableCollection<User> users;
        private ObservableCollection<Jobs> jobs;

        public ObservableCollection<User> Users
        {
            get => users;
            set
            {
                users = value;
                OnPropertyChanged(nameof(Users));
            }
        }

        public ObservableCollection<Jobs> Jobs
        {
            get => jobs;
            set
            {
                jobs = value;
                OnPropertyChanged(nameof(Jobs));
            }
        }
        public Command Refreshing { get; }
        public UsersListViewModel()
        {
            Users = new ObservableCollection<User>();
            Refreshing = new Command(LoadUsers);
            Users = new ObservableCollection<User>();
            Jobs = new ObservableCollection<Jobs>();
            SetCurrentUser();
            LoadUsers();

        }
        private async void LoadUsers()
        {
            IsBusy = true;
            Users.Clear();
            var firebase = new FirebaseDatabase();
            var currentUser = await DataStore.GetCurrentUser();

            if (currentUser.IsMechanic)
            {
                var allJobs = await firebase.GetAllJobs(currentUser.Phone);
                var users = await DataStore.GetUsersAsync();
                foreach (var item in users)
                {
                    foreach (var i in allJobs)
                    {
                        Jobs.Add(i);
                        if(item.Phone == i.ClientPhone && item.Progress != "Completed")
                        {
                            item.Progress = i.Progress;
                            Users.Add(item);
                        }
                    }
                }
            }
            else
            {
                try
                {
                    var httpClient = await HttpRequests._client.GetAsync(HttpRequests.BaseUri + "/fetch.php");
                    httpClient.EnsureSuccessStatusCode();
                    if (httpClient.IsSuccessStatusCode)
                    {
                        string response = await httpClient.Content.ReadAsStringAsync();
                        List<User> users = JsonConvert.DeserializeObject<List<User>>(response);
                        var allJobs = await firebase.GetAllJobs();
                        var mechanics = (await firebase.GetAllUsers()).FindAll(ag => ag.IsMechanic);
                        foreach (var item in users)
                        {
                            item.Progress = allJobs.Find(ag => ag.MechanicPhone == item.Phone && ag.ClientPhone == currentUser.Phone).Progress;
                            item.PhotoUrl = mechanics.Find(ag => ag.Phone == item.Phone).PhotoUrl;
                            Users.Add(item);
                        }
                    }
                
                }
                catch
                {
                    return;
                }
            }

            IsBusy = false;
        }


    }
}
