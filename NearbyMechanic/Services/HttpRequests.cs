using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace NearbyMechanic.Services
{
    public static class HttpRequests
    {
        public static HttpClient _client = new HttpClient();
        public static string BaseUri = "https://doctorsarch.org/mechanics";
        
    }
}
