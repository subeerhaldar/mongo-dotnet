using Microsoft.AspNetCore.Mvc;
using MongoGridApi.Models;
using MongoGridApi.Repositories;
using MongoGridApi.DTOs;

namespace MongoGridApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IRepository<User> _userRepository;

        public UsersController(IRepository<User> userRepository)
        {
            _userRepository = userRepository;
        }

        // GET: api/users
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var users = await _userRepository.GetAllAsync();
                var response = users.Select(u => new UserResponse
                {
                    Id = u.Id,
                    Name = u.Name,
                    Email = u.Email,
                    Address = u.Address != null ? new AddressResponse
                    {
                        Street = u.Address.Street,
                        City = u.Address.City,
                        State = u.Address.State,
                        ZipCode = u.Address.ZipCode,
                        Country = u.Address.Country
                    } : null
                });

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = "An error occurred while retrieving users", Details = ex.Message });
            }
        }

        // GET: api/users/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(id);
                if (user == null)
                {
                    return NotFound(new { Error = "User not found" });
                }

                var response = new UserResponse
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
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = "An error occurred while retrieving the user", Details = ex.Message });
            }
        }

        // POST: api/users
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Check if email already exists
                var existingUser = await _userRepository.FindAsync(u => u.Email == request.Email);
                if (existingUser.Any())
                {
                    return Conflict(new { Error = "A user with this email already exists" });
                }

                var user = new User
                {
                    Name = request.Name,
                    Email = request.Email,
                    Address = request.Address != null ? new Address
                    {
                        Street = request.Address.Street,
                        City = request.Address.City,
                        State = request.Address.State,
                        ZipCode = request.Address.ZipCode,
                        Country = request.Address.Country
                    } : null
                };

                var createdUser = await _userRepository.CreateAsync(user);

                var response = new UserResponse
                {
                    Id = createdUser.Id,
                    Name = createdUser.Name,
                    Email = createdUser.Email,
                    Address = createdUser.Address != null ? new AddressResponse
                    {
                        Street = createdUser.Address.Street,
                        City = createdUser.Address.City,
                        State = createdUser.Address.State,
                        ZipCode = createdUser.Address.ZipCode,
                        Country = createdUser.Address.Country
                    } : null
                };

                return CreatedAtAction(nameof(GetById), new { id = createdUser.Id }, response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = "An error occurred while creating the user", Details = ex.Message });
            }
        }

        // PUT: api/users/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateUserRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var existingUser = await _userRepository.GetByIdAsync(id);
                if (existingUser == null)
                {
                    return NotFound(new { Error = "User not found" });
                }

                // Check if email is being changed and if it conflicts
                if (existingUser.Email != request.Email)
                {
                    var emailConflict = await _userRepository.FindAsync(u => u.Email == request.Email && u.Id != id);
                    if (emailConflict.Any())
                    {
                        return Conflict(new { Error = "A user with this email already exists" });
                    }
                }

                existingUser.Name = request.Name;
                existingUser.Email = request.Email;
                existingUser.Address = request.Address != null ? new Address
                {
                    Street = request.Address.Street,
                    City = request.Address.City,
                    State = request.Address.State,
                    ZipCode = request.Address.ZipCode,
                    Country = request.Address.Country
                } : null;

                await _userRepository.UpdateAsync(id, existingUser);

                var response = new UserResponse
                {
                    Id = existingUser.Id,
                    Name = existingUser.Name,
                    Email = existingUser.Email,
                    Address = existingUser.Address != null ? new AddressResponse
                    {
                        Street = existingUser.Address.Street,
                        City = existingUser.Address.City,
                        State = existingUser.Address.State,
                        ZipCode = existingUser.Address.ZipCode,
                        Country = existingUser.Address.Country
                    } : null
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = "An error occurred while updating the user", Details = ex.Message });
            }
        }

        // DELETE: api/users/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(id);
                if (user == null)
                {
                    return NotFound(new { Error = "User not found" });
                }

                await _userRepository.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = "An error occurred while deleting the user", Details = ex.Message });
            }
        }
    }
}