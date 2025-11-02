using DoctorAppointmentSystem.Models;
using System.Collections.Generic;
using System.Linq;

namespace DoctorAppointmentSystem.Services
{
    public class DoctorService
    {
        private readonly List<Doctor> _doctors;

        public DoctorService()
        {
            _doctors = new List<Doctor>
            {
                new Doctor { Id = "1", Name = "Dr. A. Tarun", Specialization = "Cardiologist", Availability = "Mon-Fri, 9 AM - 5 PM" },
                new Doctor { Id = "2", Name = "Dr. Neha Kapoor", Specialization = "Dermatologist", Availability = "Tue-Sat, 10 AM - 4 PM" },
                new Doctor { Id = "3", Name = "Dr. R. Mehta", Specialization = "Dentist", Availability = "Mon-Fri, 11 AM - 6 PM" },

                // ✅ New Doctors Added Below
                new Doctor { Id = "4", Name = "Dr. Priya Sharma", Specialization = "Neurologist", Availability = "Mon-Fri, 10 AM - 4 PM" },
                new Doctor { Id = "5", Name = "Dr. Sandeep Verma", Specialization = "Orthopedic Surgeon", Availability = "Tue-Sat, 9 AM - 3 PM" },
                new Doctor { Id = "6", Name = "Dr. Kavita Rao", Specialization = "Pediatrician", Availability = "Mon-Fri, 8 AM - 2 PM" }
            };
        }

        public List<Doctor> GetAllDoctors() => _doctors;

        public Doctor GetDoctorById(string id) =>
            _doctors.FirstOrDefault(d => d.Id == id);
    }
}
