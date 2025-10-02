using Microsoft.AspNetCore.Mvc;
using MongoGridApi.Models;
using MongoGridApi.Repositories;
using MongoGridApi.DTOs;

namespace MongoGridApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Product> _productRepository;

        public OrdersController(
            IOrderRepository orderRepository,
            IRepository<User> userRepository,
            IRepository<Product> productRepository)
        {
            _orderRepository = orderRepository;
            _userRepository = userRepository;
            _productRepository = productRepository;
        }

        // GET: api/orders
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var orders = await _orderRepository.GetAllAsync();
                var users = await _userRepository.GetAllAsync();
                var products = await _productRepository.GetAllAsync();

                var userDict = users.ToDictionary(u => u.Id, u => u);
                var productDict = products.ToDictionary(p => p.Id, p => p);

                var response = orders.Select(o => new OrderResponse
                {
                    Id = o.Id,
                    UserId = o.UserId,
                    OrderDate = o.OrderDate,
                    TotalAmount = o.TotalAmount,
                    Status = o.Status,
                    User = o.UserId != null && userDict.ContainsKey(o.UserId)
                        ? new UserResponse
                        {
                            Id = userDict[o.UserId].Id,
                            Name = userDict[o.UserId].Name,
                            Email = userDict[o.UserId].Email,
                            Address = userDict[o.UserId].Address != null ? new AddressResponse
                            {
                                Street = userDict[o.UserId].Address.Street,
                                City = userDict[o.UserId].Address.City,
                                State = userDict[o.UserId].Address.State,
                                ZipCode = userDict[o.UserId].Address.ZipCode,
                                Country = userDict[o.UserId].Address.Country
                            } : null
                        }
                        : null,
                    Items = o.Items.Select(i => new OrderItemResponse
                    {
                        ProductId = i.ProductId,
                        Quantity = i.Quantity,
                        UnitPrice = i.UnitPrice,
                        Product = i.ProductId != null && productDict.ContainsKey(i.ProductId)
                            ? new ProductResponse
                            {
                                Id = productDict[i.ProductId].Id,
                                Name = productDict[i.ProductId].Name,
                                Description = productDict[i.ProductId].Description,
                                Price = productDict[i.ProductId].Price,
                                CategoryId = productDict[i.ProductId].CategoryId,
                                StockQuantity = productDict[i.ProductId].StockQuantity
                            }
                            : null
                    }).ToList()
                });

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = "An error occurred while retrieving orders", Details = ex.Message });
            }
        }

        // GET: api/orders/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            try
            {
                var order = await _orderRepository.GetByIdAsync(id);
                if (order == null)
                {
                    return NotFound(new { Error = "Order not found" });
                }

                User? user = null;
                if (!string.IsNullOrEmpty(order.UserId))
                {
                    user = await _userRepository.GetByIdAsync(order.UserId);
                }

                var products = await _productRepository.GetAllAsync();
                var productDict = products.ToDictionary(p => p.Id, p => p);

                var response = new OrderResponse
                {
                    Id = order.Id,
                    UserId = order.UserId,
                    OrderDate = order.OrderDate,
                    TotalAmount = order.TotalAmount,
                    Status = order.Status,
                    User = user != null ? new UserResponse
                    {
                        Id = user.Id,
                        Name = user.Name,
                        Email = user.Email,
                        Address = user.Address != null ? new AddressResponse
                        {
                            Street = user.Address.Street,
                            City = user.Address.City,
                            State = user.Address.State,
                            ZipCode = user.Address.ZipCode,
                            Country = user.Address.Country
                        } : null
                    } : null,
                    Items = order.Items.Select(i => new OrderItemResponse
                    {
                        ProductId = i.ProductId,
                        Quantity = i.Quantity,
                        UnitPrice = i.UnitPrice,
                        Product = i.ProductId != null && productDict.ContainsKey(i.ProductId)
                            ? new ProductResponse
                            {
                                Id = productDict[i.ProductId].Id,
                                Name = productDict[i.ProductId].Name,
                                Description = productDict[i.ProductId].Description,
                                Price = productDict[i.ProductId].Price,
                                CategoryId = productDict[i.ProductId].CategoryId,
                                StockQuantity = productDict[i.ProductId].StockQuantity
                            }
                            : null
                    }).ToList()
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = "An error occurred while retrieving the order", Details = ex.Message });
            }
        }

        // POST: api/orders
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateOrderRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Validate user exists
                var user = await _userRepository.GetByIdAsync(request.UserId);
                if (user == null)
                {
                    return BadRequest(new { Error = "Invalid user ID" });
                }

                // Validate all products exist and calculate total
                var products = await _productRepository.GetAllAsync();
                var productDict = products.ToDictionary(p => p.Id, p => p);
                decimal totalAmount = 0;

                foreach (var item in request.Items)
                {
                    if (!productDict.ContainsKey(item.ProductId))
                    {
                        return BadRequest(new { Error = $"Invalid product ID: {item.ProductId}" });
                    }
                    totalAmount += item.Quantity * item.UnitPrice;
                }

                var order = new Order
                {
                    UserId = request.UserId,
                    OrderDate = DateTime.UtcNow,
                    Items = request.Items.Select(i => new OrderItem
                    {
                        ProductId = i.ProductId,
                        Quantity = i.Quantity,
                        UnitPrice = i.UnitPrice
                    }).ToList(),
                    TotalAmount = totalAmount,
                    Status = "Pending"
                };

                var createdOrder = await _orderRepository.CreateAsync(order);

                var response = new OrderResponse
                {
                    Id = createdOrder.Id,
                    UserId = createdOrder.UserId,
                    OrderDate = createdOrder.OrderDate,
                    TotalAmount = createdOrder.TotalAmount,
                    Status = createdOrder.Status,
                    User = new UserResponse
                    {
                        Id = user.Id,
                        Name = user.Name,
                        Email = user.Email
                    },
                    Items = createdOrder.Items.Select(i => new OrderItemResponse
                    {
                        ProductId = i.ProductId,
                        Quantity = i.Quantity,
                        UnitPrice = i.UnitPrice,
                        Product = i.ProductId != null && productDict.ContainsKey(i.ProductId)
                            ? new ProductResponse
                            {
                                Id = productDict[i.ProductId].Id,
                                Name = productDict[i.ProductId].Name,
                                Price = productDict[i.ProductId].Price
                            }
                            : null
                    }).ToList()
                };

                return CreatedAtAction(nameof(GetById), new { id = createdOrder.Id }, response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = "An error occurred while creating the order", Details = ex.Message });
            }
        }

        // PUT: api/orders/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateOrderRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var existingOrder = await _orderRepository.GetByIdAsync(id);
                if (existingOrder == null)
                {
                    return NotFound(new { Error = "Order not found" });
                }

                // Validate user exists
                var user = await _userRepository.GetByIdAsync(request.UserId);
                if (user == null)
                {
                    return BadRequest(new { Error = "Invalid user ID" });
                }

                // Validate all products exist and calculate total
                var products = await _productRepository.GetAllAsync();
                var productDict = products.ToDictionary(p => p.Id, p => p);
                decimal totalAmount = 0;

                foreach (var item in request.Items)
                {
                    if (!productDict.ContainsKey(item.ProductId))
                    {
                        return BadRequest(new { Error = $"Invalid product ID: {item.ProductId}" });
                    }
                    totalAmount += item.Quantity * item.UnitPrice;
                }

                existingOrder.UserId = request.UserId;
                existingOrder.Items = request.Items.Select(i => new OrderItem
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList();
                existingOrder.TotalAmount = totalAmount;
                existingOrder.Status = request.Status;

                await _orderRepository.UpdateAsync(id, existingOrder);

                var response = new OrderResponse
                {
                    Id = existingOrder.Id,
                    UserId = existingOrder.UserId,
                    OrderDate = existingOrder.OrderDate,
                    TotalAmount = existingOrder.TotalAmount,
                    Status = existingOrder.Status,
                    User = new UserResponse
                    {
                        Id = user.Id,
                        Name = user.Name,
                        Email = user.Email
                    },
                    Items = existingOrder.Items.Select(i => new OrderItemResponse
                    {
                        ProductId = i.ProductId,
                        Quantity = i.Quantity,
                        UnitPrice = i.UnitPrice,
                        Product = i.ProductId != null && productDict.ContainsKey(i.ProductId)
                            ? new ProductResponse
                            {
                                Id = productDict[i.ProductId].Id,
                                Name = productDict[i.ProductId].Name,
                                Price = productDict[i.ProductId].Price
                            }
                            : null
                    }).ToList()
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = "An error occurred while updating the order", Details = ex.Message });
            }
        }

        // DELETE: api/orders/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var order = await _orderRepository.GetByIdAsync(id);
                if (order == null)
                {
                    return NotFound(new { Error = "Order not found" });
                }

                await _orderRepository.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = "An error occurred while deleting the order", Details = ex.Message });
            }
        }
    }
}