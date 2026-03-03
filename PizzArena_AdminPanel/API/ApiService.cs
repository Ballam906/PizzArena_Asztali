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
using PizzArena_AdminPanel.API.Product;
using PizzArena_AdminPanel.API.User;
using PizzArena_AdminPanel.API.Restaurant;


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

        //category

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

        //product
        public async Task<List<ProductDto>> GetAllProducts()
        {
            var response = await _client.GetAsync("api/Product");
            var body = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode) return new List<ProductDto>();

            var result = JsonSerializer.Deserialize<List<ProductDto>>(body, _jsonOptions);
            return result ?? new List<ProductDto>();
        }

        public async Task<ProductDto?> GetProductById(int id)
        {
            var response = await _client.GetAsync($"api/Product/GetById?id={id}");
            var body = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode) return null;

            var wrapper = JsonSerializer.Deserialize<ApiResponse<ProductDto>>(body, _jsonOptions);
            return wrapper?.Result;
        }

        public async Task<bool> CreateProduct(string name, string description, int price, bool isavailable, string imageurl, int categoryid)
        {
            var data = new
            {
                Name = name,
                Description = description,
                Price = price,
                IsAvailable = isavailable,
                Image_Url = imageurl,
                CategoryId = categoryid
            };


            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("api/Product", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateProduct(int id, string name, string description, int price, bool isavailable, string imageurl, int categoryid)
        {
            var data = new
            {
                Name = name,
                Description = description,
                Price = price,
                IsAvailable = isavailable,
                Image_Url = imageurl,
                CategoryId = categoryid
            };

            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PutAsync($"api/Product?id={id}", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteProduct(int id)
        {
            var response = await _client.DeleteAsync($"api/Product?id={id}");
            return response.IsSuccessStatusCode;
        }


        //user
        public async Task<List<UserDto>> GetAllUsers()
        {
            var response = await _client.GetAsync("api/User/GetAllUser");
            var body = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode) return new List<UserDto>();

            var result = JsonSerializer.Deserialize<List<UserDto>>(body, _jsonOptions);
            return result ?? new List<UserDto>();
        }

        public async Task<(bool ok, string? error)> DeleteUser(string userId)
        {
            var response = await _client.DeleteAsync(
                $"api/User/deleteuser?userId={Uri.EscapeDataString(userId)}"
            );

            var body = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
                return (true, null);

            return (false, string.IsNullOrWhiteSpace(body) ? "Ismeretlen hiba." : body);
        }

        public async Task<bool> CreateUser(string username, string email, string password)
        {
            var data = new
            {
                userName = username,
                email = email,
                password = password
            };


            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("api/User/register", content);
            var roleResponse = await _client.PostAsync(
            $"api/User/assignrole?UserName={username}&RoleName=Admin",
            null);
            return response.IsSuccessStatusCode;
        }

        //restaurant
        public async Task<List<RestaurantDto>> GetAllRestaurant()
        {
            var response = await _client.GetAsync("api/Restaurant/GetAll");
            var body = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode) return new List<RestaurantDto>();

            var wrapper = JsonSerializer.Deserialize<ApiResponse<List<RestaurantDto>>>(body, _jsonOptions);
            return wrapper?.Result;
        }
    }
}
