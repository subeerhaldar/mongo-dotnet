using System.ComponentModel.DataAnnotations;

namespace MongoGridApi.DTOs
{
    public class CreateOrderRequest
    {
        [Required(ErrorMessage = "User ID is required")]
        public string UserId { get; set; } = null!;

        [Required(ErrorMessage = "Order items are required")]
        [MinLength(1, ErrorMessage = "Order must contain at least one item")]
        public List<CreateOrderItemRequest> Items { get; set; } = new();
    }

    public class UpdateOrderRequest
    {
        [Required(ErrorMessage = "User ID is required")]
        public string UserId { get; set; } = null!;

        [Required(ErrorMessage = "Order items are required")]
        [MinLength(1, ErrorMessage = "Order must contain at least one item")]
        public List<UpdateOrderItemRequest> Items { get; set; } = new();

        [Required(ErrorMessage = "Order status is required")]
        [RegularExpression("^(Pending|Processing|Shipped|Delivered|Cancelled)$",
            ErrorMessage = "Status must be one of: Pending, Processing, Shipped, Delivered, Cancelled")]
        public string Status { get; set; } = "Pending";
    }

    public class CreateOrderItemRequest
    {
        [Required(ErrorMessage = "Product ID is required")]
        public string ProductId { get; set; } = null!;

        [Required(ErrorMessage = "Quantity is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Unit price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Unit price must be greater than 0")]
        public decimal UnitPrice { get; set; }
    }

    public class UpdateOrderItemRequest
    {
        [Required(ErrorMessage = "Product ID is required")]
        public string ProductId { get; set; } = null!;

        [Required(ErrorMessage = "Quantity is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Unit price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Unit price must be greater than 0")]
        public decimal UnitPrice { get; set; }
    }

    public class OrderItemResponse
    {
        public string ProductId { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public ProductResponse? Product { get; set; }
    }

    public class OrderResponse
    {
        public string? Id { get; set; }
        public string UserId { get; set; } = null!;
        public DateTime OrderDate { get; set; }
        public List<OrderItemResponse> Items { get; set; } = new();
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = "Pending";
        public UserResponse? User { get; set; }
    }
}