using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MongoGridApi.Models
{
    public class Product
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; } = null!;

        [BsonElement("description")]
        public string? Description { get; set; }

        [BsonElement("price")]
        public decimal Price { get; set; }

        [BsonElement("categoryId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string CategoryId { get; set; } = null!;

        [BsonElement("stockQuantity")]
        public int StockQuantity { get; set; }

        // Navigation property for joins
        [BsonElement("category")]
        public Category? Category { get; set; }
    }
}