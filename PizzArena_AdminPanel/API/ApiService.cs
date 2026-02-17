using PizzArena_AdminPanel.API.Category;
using PizzArena_AdminPanel.API.Login;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Net.Http.Headers;


namespace PizzArena_AdminPanel.API
{
    public class ApiService
    {
        private readonly HttpClient _client;
        private readonly JsonSerializerOptions _jsonOptions =
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        public ApiService()
        {
            _client = new HttpClient();
            _client.BaseAddress = new Uri("https://localhost:7218/");
        }


        public async Task<LoginResponse?> Login(string userName, string password)
        {
            var data = new { userName, password };
            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("api/User/loginadmin", content);
            var body = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode) return null;

            var result = JsonSerializer.Deserialize<LoginResponse>(body, _jsonOptions);

            if (!string.IsNullOrEmpty(result?.Token))
            {
                _client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", result.Token);
            }

            return result;
        }

        //kategória

        public async Task<List<CategoryDto>> GetAllCategories()
        {
            var response = await _client.GetAsync("api/Category");
            var body = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode) return new List<CategoryDto>();

            var wrapper = JsonSerializer.Deserialize<ApiResponse<List<CategoryDto>>>(body, _jsonOptions);
            return wrapper?.Result ?? new List<CategoryDto>();
        }

        public async Task<CategoryDto?> GetCategoryById(int id)
        {
            var response = await _client.GetAsync($"api/Category/GetById?id={id}");
            var body = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode) return null;

            var wrapper = JsonSerializer.Deserialize<ApiResponse<CategoryDto>>(body, _jsonOptions);
            return wrapper?.Result;
        }

        public async Task<bool> CreateCategory(string name)
        {
            var json = JsonSerializer.Serialize(name);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("api/Category", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateCategory(int id, string name)
        {
            var json = JsonSerializer.Serialize(name);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PutAsync($"api/Category?id={id}", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteCategory(int id)
        {
            var response = await _client.DeleteAsync($"api/Category?id={id}");
            return response.IsSuccessStatusCode;
        }
    }
}
