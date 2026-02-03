using PizzArena_AdminPanel.API.Login;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PizzArena_AdminPanel.API
{
    public class ApiService
    {
        private HttpClient _client = new HttpClient();

        public async Task<LoginResponse?> Login(string userName, string password)
        {
            var data = new
            {
                userName = userName,  
                password = password
            };

            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("https://localhost:7218/api/User/loginadmin", content);
            var body = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return JsonSerializer.Deserialize<LoginResponse>(
                body,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
        }
    }
}
