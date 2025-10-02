using Microsoft.AspNetCore.Mvc;
using MongoGridApi.Models;
using MongoGridApi.Repositories;
using MongoGridApi.DTOs;
using MongoDB.Driver;

namespace MongoGridApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly IRepository<Category> _categoryRepository;

        public CategoriesController(IRepository<Category> categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        // GET: api/categories
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var categories = await _categoryRepository.GetAllAsync();
                var response = categories.Select(c => new CategoryResponse
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description
                });

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = "An error occurred while retrieving categories", Details = ex.Message });
            }
        }

        // GET: api/categories/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            try
            {
                var category = await _categoryRepository.GetByIdAsync(id);
                if (category == null)
                {
                    return NotFound(new { Error = "Category not found" });
                }

                var response = new CategoryResponse
                {
                    Id = category.Id,
                    Name = category.Name,
                    Description = category.Description
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = "An error occurred while retrieving the category", Details = ex.Message });
            }
        }

        // POST: api/categories
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCategoryRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var category = new Category
                {
                    Name = request.Name,
                    Description = request.Description
                };

                var createdCategory = await _categoryRepository.CreateAsync(category);

                var response = new CategoryResponse
                {
                    Id = createdCategory.Id,
                    Name = createdCategory.Name,
                    Description = createdCategory.Description
                };

                return CreatedAtAction(nameof(GetById), new { id = createdCategory.Id }, response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = "An error occurred while creating the category", Details = ex.Message });
            }
        }

        // PUT: api/categories/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateCategoryRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var existingCategory = await _categoryRepository.GetByIdAsync(id);
                if (existingCategory == null)
                {
                    return NotFound(new { Error = "Category not found" });
                }

                existingCategory.Name = request.Name;
                existingCategory.Description = request.Description;

                await _categoryRepository.UpdateAsync(id, existingCategory);

                var response = new CategoryResponse
                {
                    Id = existingCategory.Id,
                    Name = existingCategory.Name,
                    Description = existingCategory.Description
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = "An error occurred while updating the category", Details = ex.Message });
            }
        }

        // DELETE: api/categories/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var category = await _categoryRepository.GetByIdAsync(id);
                if (category == null)
                {
                    return NotFound(new { Error = "Category not found" });
                }

                await _categoryRepository.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = "An error occurred while deleting the category", Details = ex.Message });
            }
        }
    }
}