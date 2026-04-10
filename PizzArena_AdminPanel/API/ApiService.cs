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
using PizzArena_AdminPanel.API.Order;
using PizzArena_AdminPanel.API.OrderItem;
using PizzArena_AdminPanel.API.GlobalSettings;
using PizzArena_AdminPanel.API.ChefSpecial;


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

        //category -kesz

        public async Task<List<CategoryDto>> GetAllCategories()
        {
            var response = await _client.GetAsync("api/Category");

            if (!response.IsSuccessStatusCode) return new List<CategoryDto>();

            var body = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<CategoryDto>>(body, _jsonOptions) ?? new List<CategoryDto>();
        }

        public async Task<CategoryDto?> GetCategoryById(int id)
        {
            var response = await _client.GetAsync($"api/Category/{id}");

            if (!response.IsSuccessStatusCode) return null;

            var body = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<CategoryDto>(body, _jsonOptions);
        }

        public async Task<bool> CreateCategory(string name)
        {
            var dto = new { Name = name };
            var json = JsonSerializer.Serialize(dto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("api/Category", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateCategory(int id, string name)
        {
            var dto = new { Name = name };
            var json = JsonSerializer.Serialize(dto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PutAsync($"api/Category/{id}", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteCategory(int id)
        {
            var response = await _client.DeleteAsync($"api/Category/{id}");
            return response.IsSuccessStatusCode;
        }

        //product - kész

        public async Task<List<ProductDto>> GetAllProducts()
        {
            var response = await _client.GetAsync("api/Product");
            if (!response.IsSuccessStatusCode) return new List<ProductDto>();

            var body = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<ProductDto>>(body, _jsonOptions) ?? new List<ProductDto>();
        }


        public async Task<ProductDto?> GetProductById(int id)
        {
            var response = await _client.GetAsync($"api/Product/{id}");

            if (!response.IsSuccessStatusCode) return null;

            var body = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ProductDto>(body, _jsonOptions);
        }


        public async Task<bool> CreateProduct(string name, string description, int price, bool isavailable, string imageurl, int categoryid)
        {
            var data = new
            {
                name = name,
                description = description,
                price = price,
                isAvailable = isavailable,
                image_Url = imageurl, 
                categoryId = categoryid
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
                id = id, 
                name = name,
                description = description,
                price = price,
                isAvailable = isavailable,
                image_Url = imageurl,
                categoryId = categoryid
            };

            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PutAsync("api/Product", content);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteProduct(int id)
        {
            var response = await _client.DeleteAsync($"api/Product/{id}");
            return response.IsSuccessStatusCode;
        }


        //user - kesz
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

        //public async Task<bool> CreateUser(string username, string email, string password)
        //{
        //    var data = new
        //    {
        //        userName = username,
        //        email = email,
        //        password = password
        //    };


        //    var json = JsonSerializer.Serialize(data);
        //    var content = new StringContent(json, Encoding.UTF8, "application/json");

        //    var response = await _client.PostAsync("api/User/register", content);
        //    var roleResponse = await _client.PostAsync(
        //    $"api/User/assignrole?UserName={username}&RoleName=Admin",
        //    null);
        //    return response.IsSuccessStatusCode;
        //}

        

        public async Task<(bool ok, string error)> CreateUser(string username, string email, string password)
        {
            var data = new { userName = username, email = email, password = password };
            var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("api/User/register", content);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                try
                {
                    using var doc = JsonDocument.Parse(responseBody);
                    if (doc.RootElement.TryGetProperty("message", out var msg))
                    {
                        return (false, msg.GetString()); 
                    }
                }
                catch
                {
                }
                return (false, responseBody);
            }

            return (true, null);
        }

        //restaurant - kesz
        public async Task<List<RestaurantDto>> GetAllRestaurant()
        {
            var response = await _client.GetAsync("api/Restaurant");

            if (!response.IsSuccessStatusCode) return new List<RestaurantDto>();

            var body = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<RestaurantDto>>(body, _jsonOptions) ?? new List<RestaurantDto>();
        }

        public async Task<RestaurantDto?> GetRestaurantById(int id)
        {
            var response = await _client.GetAsync($"api/Restaurant/{id}");

            if (!response.IsSuccessStatusCode) return null;

            var body = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<RestaurantDto>(body, _jsonOptions);
        }

        public async Task<bool> CreateRestaurant(string name, string description, string imageUrl, string openingHours, string address, string contactphone)
        {
            var data = new
            {
                Name = name,
                Description = description,
                ImageUrl = imageUrl,
                OpeningHours = openingHours,
                Address = address,
                contactPhone = contactphone
            };

            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("api/Restaurant", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateRestaurant(int id, string name, string description, string imageUrl, string openingHours, string address, string contactphone)
        {
            var data = new
            {
                Id = id, 
                Name = name,
                Description = description,
                ImageUrl = imageUrl,
                OpeningHours = openingHours,
                Address = address,
                contactPhone = contactphone
            };

            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PutAsync("api/Restaurant", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteRestaurant(int id)
        {
            var response = await _client.DeleteAsync($"api/Restaurant/{id}");
            return response.IsSuccessStatusCode;
        }

        //order - kesz

        public async Task<List<OrderDto>> GetAllOrder()
        {
            var response = await _client.GetAsync("api/Order");
            if (!response.IsSuccessStatusCode) return new List<OrderDto>();

            var body = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<OrderDto>>(body, _jsonOptions) ?? new List<OrderDto>();
        }

        public async Task<OrderDto?> GetOrderById(int id)
        {
            var response = await _client.GetAsync($"api/Order/{id}");

            if (!response.IsSuccessStatusCode) return null;

            var body = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<OrderDto>(body, _jsonOptions);
        }

        public async Task<bool> UpdateOrder(int id, string customername, string customeremail, string customerphone, string postalcode, string ccity, string cstreet, string cother, int status, int restaurantId)
        {
            var data = new
            {
                customerName = customername,
                customerEmail = customeremail,
                customerPhone = customerphone,
                postalCode = postalcode,
                city = ccity,
                street = cstreet,
                other = cother,
                status = status,
                restaurantId = restaurantId
            };

            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PutAsync($"api/Order/{id}", content);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteOrder(int id)
        {
            var response = await _client.DeleteAsync($"api/Order/{id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateOrderStatus(int id, int status)
        {
            // A PATCH kérésnél csak a státusz kódját küldjük el JSON-ben
            var json = JsonSerializer.Serialize(status);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // A kép alapján a végpont: api/Order/{id}/status
            var response = await _client.PatchAsync($"api/Order/{id}/status", content);

            return response.IsSuccessStatusCode;
        }


        //orderitem - kesz

        public async Task<List<OrderItemDto>> GetOrderItemsByOrderId(int orderId)
        {
            var response = await _client.GetAsync($"api/OrderItem/order/{orderId}");

            if (!response.IsSuccessStatusCode) return new List<OrderItemDto>();

            var body = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<OrderItemDto>>(body, _jsonOptions) ?? new List<OrderItemDto>();
        }

        public async Task<OrderItemDto?> GetOrderItemById(int id)
        {
            var response = await _client.GetAsync($"api/OrderItem/{id}");

            if (!response.IsSuccessStatusCode) return null;

            var body = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<OrderItemDto>(body, _jsonOptions);
        }

        public async Task<bool> CreateOrderItem(int orderId, int productId, int piece, int itemPrice)
        {
            var data = new
            {
                OrderId = orderId,
                ProductId = productId,
                Piece = piece,
                ItemPrice = itemPrice
            };

            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("api/OrderItem", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> PatchOrderItemQuantity(int id, int newQuantity)
        {
            var json = JsonSerializer.Serialize(newQuantity);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PatchAsync($"api/OrderItem/{id}/quantity", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteOrderItem(int id)
        {
            var response = await _client.DeleteAsync($"api/OrderItem/{id}");
            return response.IsSuccessStatusCode;
        }




        //global settings

        public async Task<GlobalSettingsDto?> GetGlobalSettings()
        {
            var response = await _client.GetAsync("api/GlobalSettings");
            if (!response.IsSuccessStatusCode) return null;

            var body = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<GlobalSettingsDto>(body, _jsonOptions);
        }

        public async Task<bool> UpdateGlobalSettings(string contactemail, string deliverytime, string facebook, string instagram)
        {
            var data = new
            {
                contactEmail = contactemail,
                deliveryTime = deliverytime,
                facebookUrl = facebook,
                instagramUrl = instagram
            };

            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PutAsync("api/GlobalSettings", content);

            return response.IsSuccessStatusCode;
        }

        //chefspecial - kesz

        public async Task<List<ChefSpecialDto>> GetAllChefSpecial()
        {
            var response = await _client.GetAsync("api/ChefSpecial");

            if (!response.IsSuccessStatusCode) return new List<ChefSpecialDto>();

            var body = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<ChefSpecialDto>>(body, _jsonOptions) ?? new List<ChefSpecialDto>();
        }

        public async Task<ChefSpecialDto?> GetChefSpecialById(int id)
        {
            var response = await _client.GetAsync($"api/ChefSpecial/{id}");

            if (!response.IsSuccessStatusCode) return null;

            var body = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ChefSpecialDto>(body, _jsonOptions);
        }

        public async Task<bool> CreateChefSpecial(int productid, string customnote)
        {
            var data = new
            {
                ProductId = productid,
                CustomNote = customnote
            };

            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("api/ChefSpecial", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateChefSpecial(int id, int productid, string customnote)
        {
            var data = new
            {
                Id = id, 
                ProductId = productid,
                CustomNote = customnote
            };

            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PutAsync("api/ChefSpecial", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteChefSpecial(int id)
        {
            var response = await _client.DeleteAsync($"api/ChefSpecial/{id}");
            return response.IsSuccessStatusCode;
        }
    }

}

