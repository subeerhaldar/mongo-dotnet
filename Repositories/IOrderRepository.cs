using MongoGridApi.Models;

namespace MongoGridApi.Repositories
{
    public interface IOrderRepository : IRepository<Order>
    {
        Task<IEnumerable<Order>> GetOrdersWithUserDetailsAsync();
        Task<IEnumerable<Order>> GetOrdersWithProductDetailsAsync();
        Task<IEnumerable<Order>> GetOrdersWithFullDetailsAsync();
        Task<IEnumerable<Order>> GetOrdersWithFullDetailsAsync(int page, int pageSize);
    }
}