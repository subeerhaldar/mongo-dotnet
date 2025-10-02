using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MongoGridApi.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; } = null!;

        [BsonElement("email")]
        public string Email { get; set; } = null!;

        [BsonElement("address")]
        public Address? Address { get; set; }
    }

    public class Address
    {
        [BsonElement("street")]
        public string? Street { get; set; }

        [BsonElement("city")]
        public string? City { get; set; }

        [BsonElement("state")]
        public string? State { get; set; }

        [BsonElement("zipCode")]
        public string? ZipCode { get; set; }

        [BsonElement("country")]
        public string? Country { get; set; }
    }
}