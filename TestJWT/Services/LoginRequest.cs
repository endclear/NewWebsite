
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TestJWT.Models;

namespace TestJWT.Services
{
    public interface ILoginRequest
    {
        Task<UserModel> Login(User request, string uri, string token = "");
        Task<IEnumerable<Product>> GetProducts( string uri, string token);

    }
    public class LoginRequest : ILoginRequest
    {
        private HttpClient httpClient = new HttpClient();
        public async Task<IEnumerable<Product>> GetProducts( string uri, string token)
        {
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            HttpResponseMessage response = await httpClient.GetAsync(uri);
            string responseBody = await response.Content.ReadAsStringAsync();
            var res = JsonConvert.DeserializeObject<IEnumerable<Product>>(responseBody);
            return res;
        }


        public async Task<UserModel> Login(User request, string uri, string token = "")
        {

            var json = JsonConvert.SerializeObject(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await httpClient.PostAsync(uri, content);
            string responseBody = await response.Content.ReadAsStringAsync();
            var res = JsonConvert.DeserializeObject<UserModel>(responseBody);
            return res;

        }
    }
}
