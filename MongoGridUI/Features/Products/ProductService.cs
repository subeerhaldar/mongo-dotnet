using MongoGridUI.Models;
using MongoGridUI.Services;

namespace MongoGridUI.Features.Products
{
    public class ProductService : ApiService
    {
        public ProductService(HttpClient httpClient) : base(httpClient) { }

        public async Task<List<ProductResponse>?> GetAllAsync()
        {
            return await GetAsync<List<ProductResponse>>("products");
        }

        public async Task<ProductResponse?> GetByIdAsync(string id)
        {
            return await GetAsync<ProductResponse>($"products/{id}");
        }

        public async Task<ProductResponse?> CreateAsync(CreateProductRequest request)
        {
            return await PostAsync<ProductResponse>("products", request);
        }

        public async Task UpdateAsync(string id, UpdateProductRequest request)
        {
            await PutAsync($"products/{id}", request);
            return await GetByIdAsync(id);
        }

        public async Task DeleteAsync(string id)
        {
            await DeleteAsync($"products/{id}");
        }
    }
}