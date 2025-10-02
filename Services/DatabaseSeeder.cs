using MongoDB.Driver;
using MongoGridApi.Models;

namespace MongoGridApi.Services
{
    public class DatabaseSeeder
    {
        private readonly IMongoDatabase _database;

        public DatabaseSeeder(IMongoDatabase database)
        {
            _database = database;
        }

        public async Task SeedAsync()
        {
            var random = new Random();

            // Sample data arrays
            var categoryNames = new[] { "Electronics", "Books", "Clothing", "Home & Garden", "Sports", "Automotive", "Health & Beauty", "Toys", "Food", "Office Supplies" };
            var categoryDescriptions = new[] { "Electronic devices and gadgets", "Books and publications", "Apparel and accessories", "Home improvement and garden supplies", "Sports equipment", "Car parts and accessories", "Health and beauty products", "Toys and games", "Food and beverages", "Office supplies and stationery" };

            var productNames = new[] { "Laptop", "Smartphone", "Tablet", "Headphones", "Mouse", "Keyboard", "Monitor", "Printer", "Router", "Smart Watch", "Book", "Notebook", "Pen", "T-Shirt", "Jeans", "Shoes", "Hat", "Jacket", "Garden Hose", "Lawn Mower", "Basketball", "Tennis Racket", "Dumbbells", "Yoga Mat", "Car Battery", "Oil Filter", "Tires", "Car Wash", "Shampoo", "Soap", "Toothbrush", "Action Figure", "Board Game", "Puzzle", "Coffee", "Tea", "Chocolate", "Snacks", "Paper", "Stapler" };
            var productDescriptions = new[] { "High-performance device", "Latest model available", "Premium quality product", "Durable and reliable", "Essential accessory", "Professional grade", "User-friendly design", "Energy efficient", "Wireless connectivity", "Comfortable and stylish" };

            var firstNames = new[] { "John", "Jane", "Bob", "Alice", "Charlie", "Diana", "Eve", "Frank", "Grace", "Henry", "Ivy", "Jack", "Kate", "Liam", "Mia", "Noah", "Olivia", "Peter", "Quinn", "Ryan" };
            var lastNames = new[] { "Doe", "Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia", "Miller", "Davis", "Rodriguez", "Martinez", "Hernandez", "Lopez", "Gonzalez", "Wilson", "Anderson", "Thomas", "Taylor", "Moore", "Jackson" };
            var cities = new[] { "New York", "Los Angeles", "Chicago", "Houston", "Phoenix", "Philadelphia", "San Antonio", "San Diego", "Dallas", "San Jose" };
            var states = new[] { "NY", "CA", "IL", "TX", "AZ", "PA", "FL", "OH", "GA", "NC" };
            var countries = new[] { "USA", "Canada", "UK", "Germany", "France" };

            // Seed categories
            var categoriesCollection = _database.GetCollection<Category>("categories");
            if (await categoriesCollection.CountDocumentsAsync(FilterDefinition<Category>.Empty) == 0)
            {
                var categories = new List<Category>();
                for (int i = 0; i < categoryNames.Length; i++)
                {
                    categories.Add(new Category
                    {
                        Name = categoryNames[i],
                        Description = categoryDescriptions[i]
                    });
                }
                await categoriesCollection.InsertManyAsync(categories);
            }

            // Seed products
            var productsCollection = _database.GetCollection<Product>("products");
            if (await productsCollection.CountDocumentsAsync(FilterDefinition<Product>.Empty) == 0)
            {
                var categories = await categoriesCollection.Find(FilterDefinition<Category>.Empty).ToListAsync();
                var products = new List<Product>();
                for (int i = 0; i < 1000; i++)
                {
                    var category = categories[random.Next(categories.Count)];
                    products.Add(new Product
                    {
                        Name = $"{productNames[random.Next(productNames.Length)]} {i + 1}",
                        Description = productDescriptions[random.Next(productDescriptions.Length)],
                        Price = Math.Round((decimal)(random.NextDouble() * 500 + 10), 2),
                        CategoryId = category.Id!,
                        StockQuantity = random.Next(1, 1000)
                    });
                }
                await productsCollection.InsertManyAsync(products);
            }

            // Seed users
            var usersCollection = _database.GetCollection<User>("users");
            if (await usersCollection.CountDocumentsAsync(FilterDefinition<User>.Empty) == 0)
            {
                var users = new List<User>();
                for (int i = 0; i < 500; i++)
                {
                    users.Add(new User
                    {
                        Name = $"{firstNames[random.Next(firstNames.Length)]} {lastNames[random.Next(lastNames.Length)]}",
                        Email = $"user{i + 1}@example.com",
                        Address = new Address
                        {
                            Street = $"{random.Next(1, 9999)} {new[] { "Main St", "Oak Ave", "Pine Rd", "Elm St", "Maple Dr" }[random.Next(5)]}",
                            City = cities[random.Next(cities.Length)],
                            State = states[random.Next(states.Length)],
                            ZipCode = $"{random.Next(10000, 99999)}",
                            Country = countries[random.Next(countries.Length)]
                        }
                    });
                }
                await usersCollection.InsertManyAsync(users);
            }

            // Seed orders
            var ordersCollection = _database.GetCollection<Order>("orders");
            if (await ordersCollection.CountDocumentsAsync(FilterDefinition<Order>.Empty) == 0)
            {
                var users = await usersCollection.Find(FilterDefinition<User>.Empty).ToListAsync();
                var products = await productsCollection.Find(FilterDefinition<Product>.Empty).ToListAsync();

                var orders = new List<Order>();
                for (int i = 0; i < 2000; i++)
                {
                    var user = users[random.Next(users.Count)];
                    var orderItems = new List<OrderItem>();
                    var totalAmount = 0m;
                    var itemCount = random.Next(1, 6); // 1-5 items per order

                    for (int j = 0; j < itemCount; j++)
                    {
                        var product = products[random.Next(products.Count)];
                        var quantity = random.Next(1, 5);
                        orderItems.Add(new OrderItem
                        {
                            ProductId = product.Id!,
                            Quantity = quantity,
                            UnitPrice = product.Price
                        });
                        totalAmount += quantity * product.Price;
                    }

                    orders.Add(new Order
                    {
                        UserId = user.Id!,
                        OrderDate = DateTime.UtcNow.AddDays(-random.Next(0, 365)),
                        Items = orderItems,
                        TotalAmount = Math.Round(totalAmount, 2),
                        Status = new[] { "Pending", "Processing", "Shipped", "Delivered", "Cancelled" }[random.Next(5)]
                    });
                }
                await ordersCollection.InsertManyAsync(orders);
            }

            // Create indexes for performance
            await CreateIndexesAsync();
        }

        private async Task CreateIndexesAsync()
        {
            var database = _database;

            // Products indexes
            var productsCollection = database.GetCollection<Product>("products");
            await productsCollection.Indexes.CreateOneAsync(new CreateIndexModel<Product>(
                Builders<Product>.IndexKeys.Ascending(p => p.Name)));
            await productsCollection.Indexes.CreateOneAsync(new CreateIndexModel<Product>(
                Builders<Product>.IndexKeys.Ascending(p => p.Price)));
            await productsCollection.Indexes.CreateOneAsync(new CreateIndexModel<Product>(
                Builders<Product>.IndexKeys.Ascending(p => p.CategoryId)));

            // Users indexes
            var usersCollection = database.GetCollection<User>("users");
            await usersCollection.Indexes.CreateOneAsync(new CreateIndexModel<User>(
                Builders<User>.IndexKeys.Ascending(u => u.Name)));
            await usersCollection.Indexes.CreateOneAsync(new CreateIndexModel<User>(
                Builders<User>.IndexKeys.Ascending(u => u.Email)));

            // Orders indexes
            var ordersCollection = database.GetCollection<Order>("orders");
            await ordersCollection.Indexes.CreateOneAsync(new CreateIndexModel<Order>(
                Builders<Order>.IndexKeys.Ascending(o => o.UserId)));
            await ordersCollection.Indexes.CreateOneAsync(new CreateIndexModel<Order>(
                Builders<Order>.IndexKeys.Ascending(o => o.OrderDate)));
            await ordersCollection.Indexes.CreateOneAsync(new CreateIndexModel<Order>(
                Builders<Order>.IndexKeys.Ascending(o => o.Status)));

            // Categories indexes
            var categoriesCollection = database.GetCollection<Category>("categories");
            await categoriesCollection.Indexes.CreateOneAsync(new CreateIndexModel<Category>(
                Builders<Category>.IndexKeys.Ascending(c => c.Name)));
        }
    }
}