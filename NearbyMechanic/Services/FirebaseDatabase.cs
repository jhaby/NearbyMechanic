using Firebase.Auth;
using Firebase.Database;
using Firebase.Database.Query;
using NearbyMechanic.Assets;
using NearbyMechanic.Models;
using NearbyMechanic.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using User = NearbyMechanic.Models.User;

namespace NearbyMechanic.Services
{
    public class FirebaseDatabase : BaseViewModel
    {
        private FirebaseClient firebase = new FirebaseClient(FirebaseAssets.FirebaseDBUri);

        public FirebaseDatabase()
        {
        }
        
        public async Task<User> GetUserData(string phone)
        {
            var data = (await firebase
                .Child("DriverData")
                .OnceAsync<User>())
                .Select(item => new User
                {
                    Phone = item.Object.Phone,
                    Id = item.Key,
                    Fullname = item.Object.Fullname,
                    Address = item.Object.Address,
                    IsDriver = item.Object.IsDriver,
                    IsMechanic = item.Object.IsMechanic,
                    Latitude = item.Object.Latitude,
                    Longitude = item.Object.Longitude,
                    Email = item.Object.Email,
                    PhotoUrl = item.Object.PhotoUrl,
                    CarPlateNo = item.Object.CarPlateNo,
                    CarMake = item.Object.CarMake,
                    CarModel = item.Object.CarModel
                }).ToList();

            return data.Find(a => a.Phone == phone);
        }

        public async Task<List<User>> GetAllUsers()
        {
            return (await firebase
                .Child("DriverData")
                .OnceAsync<User>())
                .Select(item => new User
                {
                    Phone = item.Object.Phone,
                    Id = item.Key,
                    Fullname = item.Object.Fullname,
                    Address = item.Object.Address,
                    IsDriver = item.Object.IsDriver,
                    IsMechanic = item.Object.IsMechanic,
                    Latitude = item.Object.Latitude,
                    Longitude = item.Object.Longitude,
                    Email = item.Object.Email,
                    PhotoUrl = item.Object.PhotoUrl,
                    CarPlateNo = item.Object.CarPlateNo,
                    CarMake = item.Object.CarMake,
                    CarModel = item.Object.CarModel
                }).ToList();
        }
        public async Task<bool> CreateNewUser(User userData)
        {
            await firebase.Child("DriverData")
                .PostAsync(userData);

            return true;
        }
        public async Task<bool> CreateNewUser(bool IsMechamic, User userData)
        {
            await firebase.Child("MechanicData")
                .PostAsync(userData);

            return true;
        }

        public async Task<bool> UpdateUser(User user)
        {
            var users = await GetAllUsers();
            var userProfile = users.FirstOrDefault(ag => ag.Phone == user.Phone);

            await firebase
                .Child("DriverData")
                .Child(userProfile.Id)
                .PutAsync(user);

            return true;
        }
        
        public async Task<bool> SubmitRequest(Jobs job)
        {
            await firebase
               .Child("Requests")
               .PostAsync(job);

            return true;
        }

        public async Task<List<Jobs>> GetAllJobs()
        {
            return (await firebase
                .Child("Requests")
                .OnceAsync<Jobs>())
                .Select(item => new Jobs
                {
                    ID = item.Key,
                    DateOfRequest = item.Object.DateOfRequest,
                    ClientPhone = item.Object.ClientPhone,
                    Status = item.Object.Status,
                    MechanicPhone = item.Object.MechanicPhone,
                    Progress = item.Object.Progress
                }).ToList();
        }
        public async Task<List<Jobs>> GetAllJobs(string phone)
        {
            var data = (await firebase
                .Child("Requests")
                .OnceAsync<Jobs>())
                .Select(item => new Jobs
                {
                    ID = item.Key,
                    DateOfRequest = item.Object.DateOfRequest,
                    ClientPhone = item.Object.ClientPhone,
                    Status = item.Object.Status,
                    MechanicPhone = item.Object.MechanicPhone,
                    Progress = item.Object.Progress
                }).ToList();

            return data.FindAll(ag => ag.MechanicPhone == phone);
        }

        public async Task<bool> UpdateJob(Jobs job)
        {
            var data = (await firebase.Child("Requests")
                .OnceAsync<Jobs>()).Where(ag => ag.Key == job.ID).FirstOrDefault();

            await firebase
                .Child("Requests")
                .Child(data.Key)
                .PutAsync(job);

            return true;
        }

    }
}
