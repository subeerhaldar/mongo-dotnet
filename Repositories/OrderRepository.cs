using MongoDB.Driver;
using MongoDB.Bson;
using MongoGridApi.Models;

namespace MongoGridApi.Repositories
{
    public class OrderRepository : Repository<Order>, IOrderRepository
    {
        private readonly IMongoDatabase _database;

        public OrderRepository(IMongoDatabase database) : base(database, "orders")
        {
            _database = database;
        }

        public async Task<IEnumerable<Order>> GetOrdersWithUserDetailsAsync()
        {
            var pipeline = new[]
            {
                new BsonDocument("$lookup", new BsonDocument
                {
                    { "from", "users" },
                    { "localField", "userId" },
                    { "foreignField", "_id" },
                    { "as", "user" }
                }),
                new BsonDocument("$unwind", new BsonDocument
                {
                    { "path", "$user" },
                    { "preserveNullAndEmptyArrays", true }
                })
            };

            var result = await _database.GetCollection<Order>("orders").Aggregate<Order>(pipeline).ToListAsync();
            return result;
        }

        public async Task<IEnumerable<Order>> GetOrdersWithProductDetailsAsync()
        {
            var pipeline = new[]
            {
                new BsonDocument("$unwind", "$items"),
                new BsonDocument("$lookup", new BsonDocument
                {
                    { "from", "products" },
                    { "localField", "items.productId" },
                    { "foreignField", "_id" },
                    { "as", "items.product" }
                }),
                new BsonDocument("$unwind", new BsonDocument
                {
                    { "path", "$items.product" },
                    { "preserveNullAndEmptyArrays", true }
                }),
                new BsonDocument("$group", new BsonDocument
                {
                    { "_id", "$_id" },
                    { "userId", new BsonDocument("$first", "$userId") },
                    { "orderDate", new BsonDocument("$first", "$orderDate") },
                    { "totalAmount", new BsonDocument("$first", "$totalAmount") },
                    { "status", new BsonDocument("$first", "$status") },
                    { "items", new BsonDocument("$push", "$items") }
                })
            };

            var result = await _database.GetCollection<Order>("orders").Aggregate<Order>(pipeline).ToListAsync();
            return result;
        }

        public async Task<IEnumerable<Order>> GetOrdersWithFullDetailsAsync()
        {
            var pipeline = new[]
            {
                // Lookup user
                new BsonDocument("$lookup", new BsonDocument
                {
                    { "from", "users" },
                    { "localField", "userId" },
                    { "foreignField", "_id" },
                    { "as", "user" }
                }),
                new BsonDocument("$unwind", new BsonDocument
                {
                    { "path", "$user" },
                    { "preserveNullAndEmptyArrays", true }
                }),
                // Unwind items
                new BsonDocument("$unwind", "$items"),
                // Lookup product
                new BsonDocument("$lookup", new BsonDocument
                {
                    { "from", "products" },
                    { "localField", "items.productId" },
                    { "foreignField", "_id" },
                    { "as", "items.product" }
                }),
                new BsonDocument("$unwind", new BsonDocument
                {
                    { "path", "$items.product" },
                    { "preserveNullAndEmptyArrays", true }
                }),
                // Lookup category for product
                new BsonDocument("$lookup", new BsonDocument
                {
                    { "from", "categories" },
                    { "localField", "items.product.categoryId" },
                    { "foreignField", "_id" },
                    { "as", "items.product.category" }
                }),
                new BsonDocument("$unwind", new BsonDocument
                {
                    { "path", "$items.product.category" },
                    { "preserveNullAndEmptyArrays", true }
                }),
                // Group back
                new BsonDocument("$group", new BsonDocument
                {
                    { "_id", "$_id" },
                    { "userId", new BsonDocument("$first", "$userId") },
                    { "user", new BsonDocument("$first", "$user") },
                    { "orderDate", new BsonDocument("$first", "$orderDate") },
                    { "totalAmount", new BsonDocument("$first", "$totalAmount") },
                    { "status", new BsonDocument("$first", "$status") },
                    { "items", new BsonDocument("$push", "$items") }
                })
            };

            var result = await _database.GetCollection<Order>("orders").Aggregate<Order>(pipeline).ToListAsync();
            return result;
        }

        public async Task<IEnumerable<Order>> GetOrdersWithFullDetailsAsync(int page, int pageSize)
        {
            var pipeline = new[]
            {
                // Lookup user
                new BsonDocument("$lookup", new BsonDocument
                {
                    { "from", "users" },
                    { "localField", "userId" },
                    { "foreignField", "_id" },
                    { "as", "user" }
                }),
                new BsonDocument("$unwind", new BsonDocument
                {
                    { "path", "$user" },
                    { "preserveNullAndEmptyArrays", true }
                }),
                // Unwind items
                new BsonDocument("$unwind", "$items"),
                // Lookup product
                new BsonDocument("$lookup", new BsonDocument
                {
                    { "from", "products" },
                    { "localField", "items.productId" },
                    { "foreignField", "_id" },
                    { "as", "items.product" }
                }),
                new BsonDocument("$unwind", new BsonDocument
                {
                    { "path", "$items.product" },
                    { "preserveNullAndEmptyArrays", true }
                }),
                // Lookup category for product
                new BsonDocument("$lookup", new BsonDocument
                {
                    { "from", "categories" },
                    { "localField", "items.product.categoryId" },
                    { "foreignField", "_id" },
                    { "as", "items.product.category" }
                }),
                new BsonDocument("$unwind", new BsonDocument
                {
                    { "path", "$items.product.category" },
                    { "preserveNullAndEmptyArrays", true }
                }),
                // Group back
                new BsonDocument("$group", new BsonDocument
                {
                    { "_id", "$_id" },
                    { "userId", new BsonDocument("$first", "$userId") },
                    { "user", new BsonDocument("$first", "$user") },
                    { "orderDate", new BsonDocument("$first", "$orderDate") },
                    { "totalAmount", new BsonDocument("$first", "$totalAmount") },
                    { "status", new BsonDocument("$first", "$status") },
                    { "items", new BsonDocument("$push", "$items") }
                }),
                // Add skip and limit for pagination
                new BsonDocument("$skip", (page - 1) * pageSize),
                new BsonDocument("$limit", pageSize)
            };

            var result = await _database.GetCollection<Order>("orders").Aggregate<Order>(pipeline).ToListAsync();
            return result;
        }
    }
}