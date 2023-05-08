using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace APIUsingTokenMongoDB.Models
{
    public class Register
    {

        public string name { get; set; }
        public bool gender { get; set; }
        
        public DateTime dob
        { get; set; }
        public string? username { get; set; }

        public string? password { get; set; }
    }
}