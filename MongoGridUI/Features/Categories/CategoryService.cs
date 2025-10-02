using MongoGridUI.Models;
using MongoGridUI.Services;

namespace MongoGridUI.Features.Categories
{
    public class CategoryService : ApiService
    {
        public CategoryService(HttpClient httpClient) : base(httpClient) { }

        public async Task<List<CategoryResponse>?> GetAllAsync()
        {
            return await GetAsync<List<CategoryResponse>>("categories");
        }

        public async Task<CategoryResponse?> GetByIdAsync(string id)
        {
            return await GetAsync<CategoryResponse>($"categories/{id}");
        }

        public async Task<CategoryResponse?> CreateAsync(CreateCategoryRequest request)
        {
            return await PostAsync<CategoryResponse>("categories", request);
        }

        public async Task UpdateAsync(string id, UpdateCategoryRequest request)
        {
            await PutAsync($"categories/{id}", request);
            return await GetByIdAsync(id);
        }

        public async Task DeleteAsync(string id)
        {
            await DeleteAsync($"categories/{id}");
        }
    }
}