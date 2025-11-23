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

       
        [HttpPost]
        public async Task<IActionResult> Register(User user)
        {
            if (string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.Password))
            {
                ViewBag.Message = "⚠️ Please fill all required fields.";
                return View();
            }

         
            var existing = await _mongoService.GetUserByEmailAsync(user.Email);
            if (existing != null)
            {
                ViewBag.Message = "⚠️ User already exists!";
                return View();
            }

            
            user.Role = "Patient";
            await _mongoService.RegisterUserAsync(user);

            ViewBag.Message = "✅ Registration successful!";
            return View();
        }


        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var user = await _mongoService.GetUserByEmailAsync(email);
            if (user == null || user.Password != HashPassword(password))
            {
                ViewBag.Message = "❌ Invalid credentials!";
                return View();
            }

            return user.Role switch
            {
                "Admin" => RedirectToAction("Dashboard", "Admin"),
                "Doctor" => RedirectToAction("Dashboard", "Doctor"),
                _ => RedirectToAction("Dashboard", "Patient")
            };
        }

        public async Task<IActionResult> Index()
        {
            var users = await _mongoService.GetAllUsersAsync();
            return View(users);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string id, User updatedUser)
        {
            await _mongoService.UpdateUserAsync(id, updatedUser);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            await _mongoService.DeleteUserAsync(id);
            return RedirectToAction("Index");
        }

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


