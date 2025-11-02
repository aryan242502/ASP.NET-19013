using DoctorAppointmentSystem.Models;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DoctorAppointmentSystem.Services
{
    public class MongoDBService
    {
        private readonly IMongoCollection<User> _users;
        private readonly IMongoCollection<Doctor> _doctors;
        private readonly IMongoCollection<Appointment> _appointments;

        // =====================================================
        // 🔗 MongoDB Connection
        // =====================================================
        public MongoDBService()
        {
            var client = new MongoClient("mongodb+srv://aryandevendra24_db_user:nRHwFSdAdYcp39BL@cluster0.7cjtk4r.mongodb.net/?retryWrites=true&w=majority&appName=Cluster0");
            var database = client.GetDatabase("DoctorAppointmentDB");


            _users = database.GetCollection<User>("Users");
            _doctors = database.GetCollection<Doctor>("Doctors");
            _appointments = database.GetCollection<Appointment>("Appointments");
        }

        // =====================================================
        // 👤 USER MANAGEMENT
        // =====================================================

        // ✅ Register new user (with password hashing)
        public async Task RegisterUserAsync(User user)
        {
            user.Password = HashPassword(user.Password);
            await _users.InsertOneAsync(user);
        }

        // ✅ Get user by email (for login & validation)
        public async Task<User> GetUserByEmailAsync(string email) =>
            await _users.Find(u => u.Email == email).FirstOrDefaultAsync();

        // ✅ Validate user login
        public async Task<User> ValidateLoginAsync(string email, string password)
        {
            string hashed = HashPassword(password);
            return await _users.Find(u => u.Email == email && u.Password == hashed).FirstOrDefaultAsync();
        }

        // ✅ Get all users (Admin view)
        public async Task<List<User>> GetAllUsersAsync() =>
            await _users.Find(_ => true).ToListAsync();

        // ✅ Update user
        public async Task UpdateUserAsync(string id, User updatedUser)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Id, id);
            await _users.ReplaceOneAsync(filter, updatedUser);
        }

        // ✅ Delete user
        public async Task DeleteUserAsync(string id) =>
            await _users.DeleteOneAsync(u => u.Id == id);


        // =====================================================
        // 👨‍⚕️ DOCTOR MANAGEMENT
        // =====================================================
        public async Task<List<Doctor>> GetAllDoctorsAsync() =>
            await _doctors.Find(_ => true).ToListAsync();

        public async Task<Doctor> GetDoctorByIdAsync(string id) =>
            await _doctors.Find(d => d.Id == id).FirstOrDefaultAsync();

        public async Task CreateDoctorAsync(Doctor doctor) =>
            await _doctors.InsertOneAsync(doctor);

        public async Task UpdateDoctorAsync(string id, Doctor updatedDoctor)
        {
            var filter = Builders<Doctor>.Filter.Eq(d => d.Id, id);
            await _doctors.ReplaceOneAsync(filter, updatedDoctor);
        }

        public async Task DeleteDoctorAsync(string id) =>
            await _doctors.DeleteOneAsync(d => d.Id == id);


        // =====================================================
        // 📅 APPOINTMENT MANAGEMENT
        // =====================================================

        // ✅ Book appointment
        public async Task BookAppointmentAsync(Appointment appointment) =>
            await _appointments.InsertOneAsync(appointment);

        public async Task<List<Appointment>> GetAppointmentsByPatientAsync(string patientId) =>
            await _appointments.Find(a => a.PatientId == patientId).ToListAsync();

        public async Task<List<Appointment>> GetAppointmentsByDoctorAsync(string doctorId) =>
            await _appointments.Find(a => a.DoctorId == doctorId).ToListAsync();

        public async Task<List<Appointment>> GetAllAppointmentsAsync() =>
            await _appointments.Find(_ => true).ToListAsync();

        public async Task UpdateAppointmentStatusAsync(string id, string newStatus)
        {
            var filter = Builders<Appointment>.Filter.Eq(a => a.Id, id);
            var update = Builders<Appointment>.Update.Set(a => a.Status, newStatus);
            await _appointments.UpdateOneAsync(filter, update);
        }


        // =====================================================
        // 🔒 Password Hash Helper
        // =====================================================
        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            StringBuilder sb = new();
            foreach (byte b in bytes)
                sb.Append(b.ToString("x2"));
            return sb.ToString();
        }
    }
}
