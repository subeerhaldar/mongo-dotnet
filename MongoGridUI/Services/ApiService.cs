using System.Net.Http.Json;
using System.Text.Json;

namespace MongoGridUI.Services
{
    public class ApiService
    {
        protected readonly HttpClient _httpClient;
        protected readonly string _baseUrl;

        public ApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _baseUrl = "https://localhost:5001/api"; // Adjust if your API is on a different port
        }

        protected async Task<T?> GetAsync<T>(string endpoint)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/{endpoint}");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<T>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GET {endpoint}: {ex.Message}");
                throw;
            }
        }

        protected async Task<T?> PostAsync<T>(string endpoint, object data)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/{endpoint}", data);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<T>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in POST {endpoint}: {ex.Message}");
                throw;
            }
        }

        protected async Task PutAsync(string endpoint, object data)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"{_baseUrl}/{endpoint}", data);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in PUT {endpoint}: {ex.Message}");
                throw;
            }
        }

        protected async Task DeleteAsync(string endpoint)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{_baseUrl}/{endpoint}");
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in DELETE {endpoint}: {ex.Message}");
                throw;
            }
        }
    }
}