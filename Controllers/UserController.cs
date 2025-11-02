using DoctorAppointmentSystem.Models;
using DoctorAppointmentSystem.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DoctorAppointmentSystem.Controllers
{
    public class UserController : Controller
    {
        private readonly MongoDBService _mongoService;

        public UserController(MongoDBService mongoService)
        {
            _mongoService = mongoService;
        }

        // ============================
        // REGISTER (GET)
        // ============================
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // ============================
        // REGISTER (POST)
        // ============================
        [HttpPost]
        public async Task<IActionResult> Register(User user)
        {
            // ✅ Server-side validation
            if (string.IsNullOrEmpty(user.FullName) ||
                string.IsNullOrEmpty(user.Email) ||
                string.IsNullOrEmpty(user.Password))
            {
                ViewBag.Message = "⚠️ Please fill all required fields.";
                return View();
            }

            // ✅ Check if user already exists
            var existing = await _mongoService.GetUserByEmailAsync(user.Email);
            if (existing != null)
            {
                ViewBag.Message = "⚠️ User already exists!";
                return View();
            }

            // ✅ Default role
            if (string.IsNullOrEmpty(user.Role))
                user.Role = "Patient";

            // ✅ INSERT USER INTO MONGODB
            await _mongoService.RegisterUserAsync(user);

            TempData["SuccessMessage"] = "✅ Registration Successful! Please login.";
            return RedirectToAction("Login");
        }

        // ============================
        // LOGIN (GET)
        // ============================
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // ============================
        // LOGIN (POST)
        // ============================
        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var user = await _mongoService.GetUserByEmailAsync(email);

            if (user == null)
            {
                ViewBag.Message = "❌ User not found!";
                return View();
            }

            if (user.Password != HashPassword(password))
            {
                ViewBag.Message = "❌ Invalid password!";
                return View();
            }

            // ✅ Redirect based on role
            return user.Role switch
            {
                "Admin" => RedirectToAction("Dashboard", "Admin"),
                "Doctor" => RedirectToAction("Dashboard", "Doctor"),
                _ => RedirectToAction("Dashboard", "Patient")
            };
        }

        // ============================
        // LIST USERS (Admin only)
        // ============================
        public async Task<IActionResult> Index()
        {
            var users = await _mongoService.GetAllUsersAsync();
            return View(users);
        }

        // ============================
        // UPDATE USER
        // ============================
        [HttpPost]
        public async Task<IActionResult> Edit(string id, User updatedUser)
        {
            await _mongoService.UpdateUserAsync(id, updatedUser);
            return RedirectToAction("Index");
        }

        // ============================
        // DELETE USER
        // ============================
        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            await _mongoService.DeleteUserAsync(id);
            return RedirectToAction("Index");
        }

        // ============================
        // HASH PASSWORD
        // ============================
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
