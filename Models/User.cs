using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DoctorAppointmentSystem.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }   // 👈 nullable to avoid null warnings

        [BsonElement("FullName")]
        public string? FullName { get; set; }

        [BsonElement("Email")]
        public string? Email { get; set; }

        [BsonElement("Password")]
        public string? Password { get; set; }

        [BsonElement("Phone")]
        public string? Phone { get; set; }

        [BsonElement("Gender")]
        public string? Gender { get; set; }

        [BsonElement("Role")]
        public string? Role { get; set; } = "Patient";  // 👈 default role
    }
}
