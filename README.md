# MongoGrid API

A .NET 8 Web API project demonstrating grid-based data retrieval from MongoDB with complex joins, LINQ queries, and performance optimizations.

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [MongoDB](https://www.mongodb.com/try/download/community) running locally on default port (27017)

## Project Structure

```
mongo-dotnet/
├── Controllers/
│   ├── GridController.cs          # API endpoints for grid data
│   ├── CategoriesController.cs    # CRUD operations for categories
│   ├── ProductsController.cs      # CRUD operations for products
│   ├── UsersController.cs         # CRUD operations for users
│   └── OrdersController.cs        # CRUD operations for orders
├── DTOs/
│   ├── CategoryDTOs.cs            # Category request/response models
│   ├── ProductDTOs.cs             # Product request/response models
│   ├── UserDTOs.cs                # User request/response models
│   └── OrderDTOs.cs               # Order request/response models
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
├── .gitignore                     # Git ignore rules
└── README.md                      # This file
```

## Features

- **Full CRUD Operations**: Complete Create, Read, Update, Delete endpoints for all collections
- **MongoDB Integration**: Uses MongoDB.Driver for NoSQL data operations
- **Complex Joins**: Implements aggregation pipelines for multi-collection joins
- **LINQ Queries**: Demonstrates LINQ-style filtering and transformation
- **Grid Support**: Paginated, sorted, and filtered endpoints for grid components
- **Repository Pattern**: Clean data access layer with dependency injection
- **Data Validation**: Comprehensive request validation with detailed error messages
- **Performance Optimized**: Connection pooling, automatic indexing, and efficient queries

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

## CRUD Operations

The API provides full Create, Read, Update, Delete operations for all collections with proper validation and error handling.

### Categories CRUD

#### Get All Categories
```
GET /api/categories
```

#### Get Category by ID
```
GET /api/categories/{id}
```

#### Create Category
```
POST /api/categories
Content-Type: application/json

{
  "name": "Electronics",
  "description": "Electronic devices and gadgets"
}
```

#### Update Category
```
PUT /api/categories/{id}
Content-Type: application/json

{
  "name": "Updated Electronics",
  "description": "Updated description"
}
```

#### Delete Category
```
DELETE /api/categories/{id}
```

### Products CRUD

#### Get All Products
```
GET /api/products
```
- Returns products with category information

#### Get Product by ID
```
GET /api/products/{id}
```

#### Create Product
```
POST /api/products
Content-Type: application/json

{
  "name": "Laptop",
  "description": "High-performance laptop",
  "price": 1200.00,
  "categoryId": "category_id_here",
  "stockQuantity": 50
}
```

#### Update Product
```
PUT /api/products/{id}
```

#### Delete Product
```
DELETE /api/products/{id}
```

### Users CRUD

#### Get All Users
```
GET /api/users
```

#### Get User by ID
```
GET /api/users/{id}
```

#### Create User
```
POST /api/users
Content-Type: application/json

{
  "name": "John Doe",
  "email": "john@example.com",
  "address": {
    "street": "123 Main St",
    "city": "Anytown",
    "state": "CA",
    "zipCode": "12345",
    "country": "USA"
  }
}
```

#### Update User
```
PUT /api/users/{id}
```

#### Delete User
```
DELETE /api/users/{id}
```

### Orders CRUD

#### Get All Orders
```
GET /api/orders
```
- Returns orders with user and product details

#### Get Order by ID
```
GET /api/orders/{id}
```

#### Create Order
```
POST /api/orders
Content-Type: application/json

{
  "userId": "user_id_here",
  "items": [
    {
      "productId": "product_id_here",
      "quantity": 2,
      "unitPrice": 25.00
    }
  ]
}
```

#### Update Order
```
PUT /api/orders/{id}
Content-Type: application/json

{
  "userId": "user_id_here",
  "items": [...],
  "status": "Shipped"
}
```

#### Delete Order
```
DELETE /api/orders/{id}
```

## Validation and Error Handling

All CRUD endpoints include:
- **Input Validation**: Data annotations for required fields, string lengths, ranges, and email formats
- **Business Logic Validation**: Foreign key validation, uniqueness constraints
- **Error Responses**: Structured error messages with appropriate HTTP status codes
- **Model State Validation**: Automatic validation of request models

### Common HTTP Status Codes
- `200 OK`: Successful GET/PUT operations
- `201 Created`: Successful POST operations
- `204 No Content`: Successful DELETE operations
- `400 Bad Request`: Validation errors or invalid data
- `404 Not Found`: Resource not found
- `409 Conflict`: Unique constraint violations (e.g., duplicate email)
- `500 Internal Server Error`: Server-side errors

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

### Backend API
1. **Start MongoDB**: Ensure MongoDB is running locally on port 27017 (default)
2. **Navigate to API project**:
   ```bash
   cd /path/to/mongo-dotnet
   ```
3. **Run the API**:
   ```bash
   dotnet run
   ```
4. **Access APIs**:
   - Swagger UI: `https://localhost:5001/swagger`
   - API endpoints: `https://localhost:5001/api/*`

The API will automatically seed large test dataset (1000+ products, 500+ users, 2000+ orders) on first run.

### Frontend UI (Optional)
1. **Navigate to UI project**:
   ```bash
   cd MongoGridUI
   ```
2. **Run the Blazor WebAssembly app**:
   ```bash
   dotnet run
   ```
3. **Access UI**: `https://localhost:5000` (or the port shown in terminal)

The UI demonstrates data grid functionality and connects to the API backend.

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
