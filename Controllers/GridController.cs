using Microsoft.AspNetCore.Mvc;
using MongoGridApi.Models;
using MongoGridApi.Repositories;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace MongoGridApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GridController : ControllerBase
    {
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IMongoDatabase _database;

        public GridController(
            IRepository<Product> productRepository,
            IRepository<User> userRepository,
            IOrderRepository orderRepository,
            IMongoDatabase database)
        {
            _productRepository = productRepository;
            _userRepository = userRepository;
            _orderRepository = orderRepository;
            _database = database;
        }

        // GET: api/grid/products
        [HttpGet("products")]
        public async Task<IActionResult> GetProducts(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? sortBy = null,
            [FromQuery] string? filter = null)
        {
            try
            {
                var query = _database.GetCollection<Product>("products").AsQueryable();

                // Apply filtering
                if (!string.IsNullOrEmpty(filter))
                {
                    query = query.Where(p => p.Name.Contains(filter) || p.Description!.Contains(filter));
                }

                // Apply sorting
                if (!string.IsNullOrEmpty(sortBy))
                {
                    switch (sortBy.ToLower())
                    {
                        case "name":
                            query = query.OrderBy(p => p.Name);
                            break;
                        case "price":
                            query = query.OrderBy(p => p.Price);
                            break;
                        default:
                            query = query.OrderBy(p => p.Name);
                            break;
                    }
                }

                // Get total count
                var totalCount = await query.CountAsync();

                // Apply pagination
                var products = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return Ok(new
                {
                    Data = products,
                    TotalCount = totalCount,
                    Page = page,
                    PageSize = pageSize
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = "An error occurred while retrieving products", Details = ex.Message });
            }
        }

        // GET: api/grid/users
        [HttpGet("users")]
        public async Task<IActionResult> GetUsers(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? sortBy = null,
            [FromQuery] string? filter = null)
        {
            var query = _database.GetCollection<User>("users").AsQueryable();

            // Apply filtering
            if (!string.IsNullOrEmpty(filter))
            {
                query = query.Where(u => u.Name.Contains(filter) || u.Email.Contains(filter));
            }

            // Apply sorting
            if (!string.IsNullOrEmpty(sortBy))
            {
                switch (sortBy.ToLower())
                {
                    case "name":
                        query = query.OrderBy(u => u.Name);
                        break;
                    case "email":
                        query = query.OrderBy(u => u.Email);
                        break;
                    default:
                        query = query.OrderBy(u => u.Name);
                        break;
                }
            }

            var totalCount = await query.CountAsync();
            var users = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new
            {
                Data = users,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            });
        }

        // GET: api/grid/orders
        [HttpGet("orders")]
        public async Task<IActionResult> GetOrders(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? sortBy = null,
            [FromQuery] string? filter = null,
            [FromQuery] bool includeDetails = false)
        {
            IEnumerable<Order> orders;

            if (includeDetails)
            {
                orders = await _orderRepository.GetOrdersWithFullDetailsAsync();
            }
            else
            {
                orders = await _orderRepository.GetAllAsync();
            }

            // Apply filtering (simple in-memory for demo)
            if (!string.IsNullOrEmpty(filter))
            {
                orders = orders.Where(o => o.Status.Contains(filter) || o.User?.Name.Contains(filter) == true);
            }

            // Apply sorting
            if (!string.IsNullOrEmpty(sortBy))
            {
                switch (sortBy.ToLower())
                {
                    case "date":
                        orders = orders.OrderBy(o => o.OrderDate);
                        break;
                    case "total":
                        orders = orders.OrderBy(o => o.TotalAmount);
                        break;
                    default:
                        orders = orders.OrderBy(o => o.OrderDate);
                        break;
                }
            }

            var totalCount = orders.Count();
            orders = orders
                .Skip((page - 1) * pageSize)
                .Take(pageSize);

            return Ok(new
            {
                Data = orders,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            });
        }

        // GET: api/grid/orders/joined
        [HttpGet("orders/joined")]
        public async Task<IActionResult> GetOrdersWithJoins(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                // Get paginated results
                var orders = await _orderRepository.GetOrdersWithFullDetailsAsync(page, pageSize);

                // Get total count for pagination info (this could be optimized)
                var totalCount = await _orderRepository.CountAsync();

                return Ok(new
                {
                    Data = orders,
                    TotalCount = totalCount,
                    Page = page,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = "An error occurred while retrieving joined orders", Details = ex.Message });
            }
        }
    }
}