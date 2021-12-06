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
    public class SupplierViewModel : BaseViewModel
    {
        public SupplierViewModel()
        {
            LoadData();
            RefreshDataCommand = new Command(LoadData);
        }
        private ObservableCollection<User> suppliers;

        public ObservableCollection<User> Suppliers
        {
            get { return suppliers; }
            set
            {
                suppliers = value; 
                OnPropertyChanged(nameof(Suppliers));
            }
        }
        public Command RefreshDataCommand { get; }
        private async void LoadData()
        {
            IsBusy = true;
            try
            {
                var httpClient = await HttpRequests._client.GetAsync(HttpRequests.BaseUri + "/fetch.php");
                httpClient.EnsureSuccessStatusCode();
                if (httpClient.IsSuccessStatusCode)
                {
                    string response = await httpClient.Content.ReadAsStringAsync();
                    List<User> users = JsonConvert.DeserializeObject<List<User>>(response);
                    Suppliers.Clear();
                    foreach (var item in users)
                    {
                        Suppliers.Add(item);
                    }
                }

            }
            catch
            {
                return;
            }
            IsBusy = false;
        }
    }
}
