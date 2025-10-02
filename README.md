# MongoGrid API

A .NET 8 Web API project demonstrating grid-based data retrieval from MongoDB with complex joins, LINQ queries, and performance optimizations.

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [MongoDB](https://www.mongodb.com/try/download/community) running locally on default port (27017)

## Project Structure

### Backend API (mongo-dotnet/)
```
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

### Frontend UI (MongoGridUI/)
```
├── Features/
│   ├── Categories/
│   │   ├── CategoryService.cs     # API service for categories
│   │   └── CategoriesPage.razor   # Categories management page
│   ├── Products/
│   │   ├── ProductService.cs      # API service for products
│   │   └── ProductsPage.razor     # Products management page
│   ├── Users/                     # (Extensible for users)
│   └── Orders/                    # (Extensible for orders)
├── Shared/
│   ├── Components/
│   │   ├── AdvancedDataGrid.razor    # Full-featured data grid
│   │   ├── DynamicDropdown.razor     # Generic dropdown component
│   │   ├── CrudForm.razor            # Modal CRUD forms
│   │   └── VirtualizedDataGrid.razor # Performance-optimized grid
│   └── Services/
│       └── ApiService.cs          # Base API service
├── Models/                        # UI-specific DTOs
├── Layout/
│   ├── MainLayout.razor          # Main application layout
│   └── NavMenu.razor             # Navigation sidebar
├── _Imports.razor                # Global using directives
├── App.razor                     # Root component
├── Program.cs                    # Application entry point
└── wwwroot/                      # Static assets
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

### Frontend UI
1. **Navigate to UI project**:
   ```bash
   cd MongoGridUI
   ```
2. **Run the Blazor WebAssembly app**:
   ```bash
   dotnet run
   ```
3. **Access UI**: `http://localhost:5265` (or assigned port)

#### UI Features:
- **Categories Page** (`/categories`): Full CRUD operations with data grid
- **Products Page** (`/products`): CRUD with dynamic dropdown binding to categories
- **Advanced Components**:
  - `AdvancedDataGrid`: Pagination, sorting, filtering, search
  - `DynamicDropdown`: NoSQL master data binding
  - `CrudForm`: Modal forms with validation
  - `VirtualizedDataGrid`: Performance optimization for large datasets

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

## Frontend Architecture (Blazor WebAssembly)

The project includes a comprehensive Blazor WebAssembly frontend that demonstrates modern SPA development with NoSQL data binding.

### Feature-Based Structure
```
MongoGridUI/
├── Features/
│   ├── Categories/
│   │   ├── CategoryService.cs
│   │   └── CategoriesPage.razor
│   ├── Products/
│   │   ├── ProductService.cs
│   │   └── ProductsPage.razor
│   ├── Users/
│   └── Orders/
├── Shared/
│   ├── Components/
│   │   ├── AdvancedDataGrid.razor
│   │   ├── DynamicDropdown.razor
│   │   ├── CrudForm.razor
│   │   └── VirtualizedDataGrid.razor
│   └── Services/
│       └── ApiService.cs
├── Models/
│   └── DTOs for API communication
└── Layout/
    ├── MainLayout.razor
    └── NavMenu.razor
```

### Key Components

#### AdvancedDataGrid
- **Pagination**: Server-side pagination with customizable page sizes (10, 25, 50, 100)
- **Sorting**: Column-based sorting (client-side for demo, can be server-side)
- **Filtering**: Real-time search with debounced input
- **CRUD Actions**: Edit/Delete buttons with confirmation dialogs
- **Loading States**: Spinner indicators during data operations
- **Empty States**: User-friendly messages when no data is available

#### DynamicDropdown
- **Generic Type Support**: Works with any data type using generics
- **Value/Display Selectors**: Flexible binding to NoSQL data properties
- **Validation**: Built-in required field validation with error messages
- **Bootstrap Styling**: Consistent UI design with form controls
- **Master Data Binding**: Automatically populates from API endpoints

#### CrudForm
- **Modal Interface**: Non-blocking form interactions using Bootstrap modals
- **Validation**: DataAnnotations validation with comprehensive error display
- **Loading States**: Submit button with loading indicators and disabled state
- **Generic Support**: Works with any model type through generics
- **Two-Way Binding**: Reactive form updates with `@bind` directives

#### VirtualizedDataGrid
- **Performance**: Handles thousands of records efficiently using Blazor's Virtualize component
- **Virtual Scrolling**: Only renders visible items (50px item height, 5 overscan)
- **Sticky Headers**: Fixed column headers during scroll navigation
- **Configurable Item Size**: Optimized for different row heights
- **Memory Efficient**: Prevents UI freezing with large datasets

### HttpClient Integration
- **Base Configuration**: Centralized API base URL configuration
- **Error Handling**: Comprehensive exception handling with user feedback
- **JSON Serialization**: Automatic request/response serialization
- **Dependency Injection**: Scoped service registration for proper lifecycle management
- **Typed Services**: Feature-specific services extending base ApiService

### Data Binding & Validation
- **Two-Way Binding**: `@bind` and `@bind-Value` for reactive UI updates
- **Form Validation**: `EditForm` with `DataAnnotationsValidator` for model validation
- **Error Messages**: `ValidationMessage` components for field-level errors
- **Real-time Feedback**: Immediate validation feedback as users type
- **Custom Validation**: Business logic validation through custom attributes

### Performance Optimizations
- **Virtual Scrolling**: Efficient rendering of large datasets
- **Lazy Loading**: Components load data only when needed
- **Debounced Search**: Prevents excessive API calls during typing
- **Pagination**: Server-side pagination reduces data transfer
- **Component Lifecycle**: Proper OnInitializedAsync and disposal patterns

### Navigation & Routing
- **Feature-Based Routes**: Clean URL structure (`/categories`, `/products`, etc.)
- **Sidebar Navigation**: Bootstrap-based responsive navigation menu
- **Active Link Styling**: Visual indication of current page
- **Route Parameters**: Support for ID-based routes (future enhancement)

### UI/UX Features
- **Bootstrap Styling**: Professional, responsive design
- **Loading Indicators**: Visual feedback during async operations
- **Error Handling**: User-friendly error messages and recovery
- **Responsive Design**: Mobile-friendly layouts
- **Accessibility**: Proper ARIA labels and semantic HTML

This frontend demonstrates enterprise-level Blazor development patterns suitable for large-scale applications with complex data requirements.
