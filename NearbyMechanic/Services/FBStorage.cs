using System;
using System.Collections.Generic;
using System.Text;
using Firebase.Storage;
using System.Threading.Tasks;
using System.IO;
using Xamarin.Essentials;
using Firebase.Auth;
using NearbyMechanic.Assets;
using NearbyMechanic.ViewModels;

namespace NearbyMechanic.Services
{
    public class FBStorage : BaseViewModel
    {
        private FirebaseStorage firebase;

        public FBStorage()
        {
            firebase = new FirebaseStorage(FirebaseAssets.FirebaseStorageUri);
        }

        public async Task<string> SaveImageAsync(Stream imgStream, string location, string uri)
        {
            var storageImage = await firebase
                .Child("Images")
                .Child(location)
                .Child(uri + ".jpg")
                .PutAsync(imgStream);

            return storageImage;
        }
    }
}
