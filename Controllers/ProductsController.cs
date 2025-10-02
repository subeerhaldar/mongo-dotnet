using Microsoft.AspNetCore.Mvc;
using MongoGridApi.Models;
using MongoGridApi.Repositories;
using MongoGridApi.DTOs;
using MongoDB.Driver;

namespace MongoGridApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<Category> _categoryRepository;

        public ProductsController(
            IRepository<Product> productRepository,
            IRepository<Category> categoryRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }

        // GET: api/products
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var products = await _productRepository.GetAllAsync();
                var categories = await _categoryRepository.GetAllAsync();
                var categoryDict = categories.ToDictionary(c => c.Id, c => c);

                var response = products.Select(p => new ProductResponse
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    CategoryId = p.CategoryId,
                    StockQuantity = p.StockQuantity,
                    Category = p.CategoryId != null && categoryDict.ContainsKey(p.CategoryId)
                        ? new CategoryResponse
                        {
                            Id = categoryDict[p.CategoryId].Id,
                            Name = categoryDict[p.CategoryId].Name,
                            Description = categoryDict[p.CategoryId].Description
                        }
                        : null
                });

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = "An error occurred while retrieving products", Details = ex.Message });
            }
        }

        // GET: api/products/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            try
            {
                var product = await _productRepository.GetByIdAsync(id);
                if (product == null)
                {
                    return NotFound(new { Error = "Product not found" });
                }

                Category? category = null;
                if (!string.IsNullOrEmpty(product.CategoryId))
                {
                    category = await _categoryRepository.GetByIdAsync(product.CategoryId);
                }

                var response = new ProductResponse
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    CategoryId = product.CategoryId,
                    StockQuantity = product.StockQuantity,
                    Category = category != null ? new CategoryResponse
                    {
                        Id = category.Id,
                        Name = category.Name,
                        Description = category.Description
                    } : null
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = "An error occurred while retrieving the product", Details = ex.Message });
            }
        }

        // POST: api/products
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProductRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Validate category exists
                var category = await _categoryRepository.GetByIdAsync(request.CategoryId);
                if (category == null)
                {
                    return BadRequest(new { Error = "Invalid category ID" });
                }

                var product = new Product
                {
                    Name = request.Name,
                    Description = request.Description,
                    Price = request.Price,
                    CategoryId = request.CategoryId,
                    StockQuantity = request.StockQuantity
                };

                var createdProduct = await _productRepository.CreateAsync(product);

                var response = new ProductResponse
                {
                    Id = createdProduct.Id,
                    Name = createdProduct.Name,
                    Description = createdProduct.Description,
                    Price = createdProduct.Price,
                    CategoryId = createdProduct.CategoryId,
                    StockQuantity = createdProduct.StockQuantity,
                    Category = new CategoryResponse
                    {
                        Id = category.Id,
                        Name = category.Name,
                        Description = category.Description
                    }
                };

                return CreatedAtAction(nameof(GetById), new { id = createdProduct.Id }, response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = "An error occurred while creating the product", Details = ex.Message });
            }
        }

        // PUT: api/products/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateProductRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var existingProduct = await _productRepository.GetByIdAsync(id);
                if (existingProduct == null)
                {
                    return NotFound(new { Error = "Product not found" });
                }

                // Validate category exists
                var category = await _categoryRepository.GetByIdAsync(request.CategoryId);
                if (category == null)
                {
                    return BadRequest(new { Error = "Invalid category ID" });
                }

                existingProduct.Name = request.Name;
                existingProduct.Description = request.Description;
                existingProduct.Price = request.Price;
                existingProduct.CategoryId = request.CategoryId;
                existingProduct.StockQuantity = request.StockQuantity;

                await _productRepository.UpdateAsync(id, existingProduct);

                var response = new ProductResponse
                {
                    Id = existingProduct.Id,
                    Name = existingProduct.Name,
                    Description = existingProduct.Description,
                    Price = existingProduct.Price,
                    CategoryId = existingProduct.CategoryId,
                    StockQuantity = existingProduct.StockQuantity,
                    Category = new CategoryResponse
                    {
                        Id = category.Id,
                        Name = category.Name,
                        Description = category.Description
                    }
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = "An error occurred while updating the product", Details = ex.Message });
            }
        }

        // DELETE: api/products/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var product = await _productRepository.GetByIdAsync(id);
                if (product == null)
                {
                    return NotFound(new { Error = "Product not found" });
                }

                await _productRepository.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = "An error occurred while deleting the product", Details = ex.Message });
            }
        }
    }
}