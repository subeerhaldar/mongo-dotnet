using System.ComponentModel.DataAnnotations;

namespace MongoGridUI.Models
{
    public class CreateUserRequest
    {
        [Required(ErrorMessage = "User name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "User name must be between 2 and 100 characters")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [StringLength(254, ErrorMessage = "Email cannot exceed 254 characters")]
        public string Email { get; set; } = null!;

        public CreateAddressRequest? Address { get; set; }
    }

    public class UpdateUserRequest
    {
        [Required(ErrorMessage = "User name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "User name must be between 2 and 100 characters")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [StringLength(254, ErrorMessage = "Email cannot exceed 254 characters")]
        public string Email { get; set; } = null!;

        public UpdateAddressRequest? Address { get; set; }
    }

    public class CreateAddressRequest
    {
        [StringLength(200, ErrorMessage = "Street cannot exceed 200 characters")]
        public string? Street { get; set; }

        [StringLength(100, ErrorMessage = "City cannot exceed 100 characters")]
        public string? City { get; set; }

        [StringLength(50, ErrorMessage = "State cannot exceed 50 characters")]
        public string? State { get; set; }

        [StringLength(20, ErrorMessage = "Zip code cannot exceed 20 characters")]
        public string? ZipCode { get; set; }

        [StringLength(50, ErrorMessage = "Country cannot exceed 50 characters")]
        public string? Country { get; set; }
    }

    public class UpdateAddressRequest
    {
        [StringLength(200, ErrorMessage = "Street cannot exceed 200 characters")]
        public string? Street { get; set; }

        [StringLength(100, ErrorMessage = "City cannot exceed 100 characters")]
        public string? City { get; set; }

        [StringLength(50, ErrorMessage = "State cannot exceed 50 characters")]
        public string? State { get; set; }

        [StringLength(20, ErrorMessage = "Zip code cannot exceed 20 characters")]
        public string? ZipCode { get; set; }

        [StringLength(50, ErrorMessage = "Country cannot exceed 50 characters")]
        public string? Country { get; set; }
    }

    public class AddressResponse
    {
        public string? Street { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? ZipCode { get; set; }
        public string? Country { get; set; }
    }

    public class UserResponse
    {
        public string? Id { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public AddressResponse? Address { get; set; }
    }
}