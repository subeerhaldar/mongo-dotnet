using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MongoGridApi.Models
{
    public class Order
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("userId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string UserId { get; set; } = null!;

        [BsonElement("orderDate")]
        public DateTime OrderDate { get; set; }

        [BsonElement("items")]
        public List<OrderItem> Items { get; set; } = new();

        [BsonElement("totalAmount")]
        public decimal TotalAmount { get; set; }

        [BsonElement("status")]
        public string Status { get; set; } = "Pending";

        // Navigation properties for joins
        [BsonElement("user")]
        public User? User { get; set; }
    }

    public class OrderItem
    {
        [BsonElement("productId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ProductId { get; set; } = null!;

        [BsonElement("quantity")]
        public int Quantity { get; set; }

        [BsonElement("unitPrice")]
        public decimal UnitPrice { get; set; }

        // Navigation property
        [BsonElement("product")]
        public Product? Product { get; set; }
    }
}