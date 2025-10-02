# MongoGrid API

A .NET 8 Web API project demonstrating grid-based data retrieval from MongoDB with complex joins, LINQ queries, and performance optimizations.

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [MongoDB](https://www.mongodb.com/try/download/community) running locally on default port (27017)

## Project Structure

```
mongo-dotnet/
├── Controllers/
│   └── GridController.cs          # API endpoints for grid data
├── Models/
│   ├── Category.cs                # Category entity
│   ├── Product.cs                 # Product entity
│   ├── User.cs                    # User entity
│   ├── Order.cs                   # Order and OrderItem entities
│   └── MongoDBSettings.cs         # MongoDB configuration
├── Repositories/
│   ├── IRepository.cs             # Generic repository interface
│   ├── Repository.cs              # Generic repository implementation
│   ├── IOrderRepository.cs        # Order-specific repository interface
│   └── OrderRepository.cs         # Order repository with joins
├── Services/
│   └── DatabaseSeeder.cs          # Sample data seeding
├── appsettings.json               # Application configuration
├── Program.cs                     # Application entry point
└── README.md                      # This file
```

## Features

- **MongoDB Integration**: Uses MongoDB.Driver for NoSQL data operations
- **Complex Joins**: Implements aggregation pipelines for multi-collection joins
- **LINQ Queries**: Demonstrates LINQ-style filtering and transformation
- **Grid Support**: Paginated, sorted, and filtered endpoints for grid components
- **Repository Pattern**: Clean data access layer with dependency injection
- **Performance Optimized**: Connection pooling, indexing recommendations, and efficient queries

## Architecture

### Models
- `Category`: Product categories
- `Product`: Items with category references
- `User`: Customers with address information
- `Order`: Purchase orders with embedded items

### Data Access
- Generic `IRepository<T>` for basic CRUD operations
- Specialized `IOrderRepository` for complex order queries with joins
- Repository implementations using MongoDB.Driver

### API Endpoints

#### Products Grid
```
GET /api/grid/products?page=1&pageSize=10&sortBy=name&filter=laptop
```
- Supports pagination, sorting by name/price, and text filtering

#### Users Grid
```
GET /api/grid/users?page=1&pageSize=10&sortBy=name&filter=john
```
- Supports pagination, sorting by name/email, and text filtering

#### Orders Grid
```
GET /api/grid/orders?page=1&pageSize=10&sortBy=date&includeDetails=true
```
- Supports pagination, sorting by date/total, and optional full details with joins

#### Orders with Joins
```
GET /api/grid/orders/joined?page=1&pageSize=10
```
- Returns paginated orders with user, product, and category details using aggregation pipelines
- Supports pagination to handle large datasets efficiently

## MongoDB Schema Design

Collections are designed for efficient queries:
- Embedded documents for order items (frequently accessed together)
- References for relationships (categories, users)
- ObjectId for MongoDB native identifiers

## Performance Best Practices

### Connection Management
- Singleton `IMongoClient` for connection pooling
- Scoped database instances per request

### Indexing
For optimal performance, create these indexes:
```javascript
// Products collection
db.products.createIndex({ "name": 1 })
db.products.createIndex({ "price": 1 })
db.products.createIndex({ "categoryId": 1 })

// Users collection
db.users.createIndex({ "name": 1 })
db.users.createIndex({ "email": 1 })

// Orders collection
db.orders.createIndex({ "userId": 1 })
db.orders.createIndex({ "orderDate": 1 })
db.orders.createIndex({ "status": 1 })

// Categories collection
db.categories.createIndex({ "name": 1 })
```

### Query Optimization
- Use LINQ for server-side filtering and sorting
- Implement pagination to limit result sets
- Leverage aggregation pipelines for complex joins
- Avoid N+1 queries by using lookups in pipelines
- Apply pagination at the database level for joined queries to prevent memory issues
- Automatic index creation on frequently queried fields (userId, productId, categoryId, etc.)

### Scalability Considerations
- Horizontal scaling with MongoDB sharding
- Read replicas for read-heavy workloads
- Connection pooling reduces overhead
- Efficient serialization with BsonIgnore for navigation properties

## Configuration

Update `appsettings.json`:
```json
{
  "MongoDB": {
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "MongoGridDb"
  }
}
```

## Running the Project

1. **Start MongoDB**: Ensure MongoDB is running locally on port 27017 (default)
2. **Update Configuration**: Modify `appsettings.json` if your MongoDB connection differs
3. **Restore Dependencies**:
   ```bash
   dotnet restore
   ```
4. **Run the Application**:
   ```bash
   dotnet run
   ```
5. **Access APIs**:
   - Swagger UI: `https://localhost:5001/swagger`
   - Health check: `https://localhost:5001/health` (if implemented)

The application will automatically seed sample data on first run.

## Sample Data

The application seeds realistic sample data on startup for performance testing:
- **10 categories** with various product types (Electronics, Books, Clothing, etc.)
- **1000 products** distributed across categories with random pricing ($10-$510) and stock levels
- **500 users** with diverse names, emails, and addresses across multiple countries
- **2000 orders** with 1-5 items each, spanning the past year with various statuses

## LINQ Query Examples

The project demonstrates various LINQ patterns:

```csharp
// Filtering and sorting
var query = _database.GetCollection<Product>("products").AsQueryable()
    .Where(p => p.Name.Contains(filter))
    .OrderBy(p => p.Name)
    .Skip((page - 1) * pageSize)
    .Take(pageSize);
```

## Aggregation Pipeline Joins

Complex joins are implemented using MongoDB aggregation:

```csharp
var pipeline = new[]
{
    new BsonDocument("$lookup", new BsonDocument
    {
        { "from", "users" },
        { "localField", "userId" },
        { "foreignField", "_id" },
        { "as", "user" }
    }),
    // Additional stages for unwinding and grouping
};
```

This approach provides efficient server-side joins without multiple round trips.
