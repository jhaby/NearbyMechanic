using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NearbyMechanic.Services
{
    public interface IDataStore<T>
    {
        Task<T> GetUserAsync(string id);
        Task<List<T>> GetUsersAsync(bool forceRefresh = false);
        void SetCurrentUser(T item);
        Task<T> GetCurrentUser();
        Task<dynamic> GetFirebaseCredentials();
    }
}
