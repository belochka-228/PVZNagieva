using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace WPF_Nagieva.Services
{
    internal class ApiClient

    {
        public static readonly HttpClient Client = new HttpClient
        {
            BaseAddress = new System.Uri("http://localhost:5067/")
        };

        public static void SetToken(string token)
        {
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        public static void Logout()
        {
            Client.DefaultRequestHeaders.Authorization = null;
        }
    }
}

