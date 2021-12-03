using NearbyMechanic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NearbyMechanic.Services
{
    public class DataStore : IDataStore<User>
    {
        private List<User> users;
        private User CurrentUser { get; set; }
        private string email = "admin@gmail.com";
        private readonly string password = "1234567890";
        private FirebaseDatabase firebase;

        public DataStore()
        {
            users = new List<User>();
            CurrentUser = new User();
            firebase = new FirebaseDatabase();

        }

        public async Task<User> GetUserAsync(string id)
        {
            return await Task.FromResult(users.FirstOrDefault(s => s.Id == id));
        }

        public async Task<List<User>> GetUsersAsync(bool forceRefresh = false)
        {
            return await firebase.GetAllUsers();
        }

        public void SetCurrentUser(User item)
        {
            CurrentUser = item;
        }

        public async Task<User> GetCurrentUser()
        {
            return await Task.FromResult(CurrentUser);
        }

        public async Task<dynamic> GetFirebaseCredentials()
        {
            return await Task.FromResult(new
            {
                Email = email,
                Password = password
            });
        }
    }
}