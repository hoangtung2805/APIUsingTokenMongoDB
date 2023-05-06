using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace APIUsingTokenMongoDB.Models
{
    public class Student
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string name { get; set; }
        public bool gender { get; set; }
        public DateTime dob
        { get; set; }
        public string? username { get; set; }

        public string? password { get; set; }
    }
}
