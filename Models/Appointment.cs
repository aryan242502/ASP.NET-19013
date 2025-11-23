using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace DoctorAppointmentSystem.Models
{
    public class Appointment
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        public string DoctorId { get; set; } = string.Empty;

        public string DoctorName { get; set; } = string.Empty;

        public string PatientId { get; set; } = string.Empty;

        public string PatientName { get; set; } = string.Empty;

        public string PatientEmail { get; set; } = string.Empty;

        public DateTime AppointmentDate { get; set; } = DateTime.Now;

        public string TimeSlot { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string Status { get; set; } = "Pending";
    }
}
