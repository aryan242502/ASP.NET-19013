using DoctorAppointmentSystem.Models;
using DoctorAppointmentSystem.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace DoctorAppointmentSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly MongoDBService _mongoService;

        public AccountController(MongoDBService mongoService)
        {
            _mongoService = mongoService;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(User user)
        {
            if (ModelState.IsValid)
            {
                user.Role = "Patient";
                await _mongoService.RegisterUserAsync(user);

                TempData["Success"] = "✅ Registration Successful! Please login now.";
                return RedirectToAction("Login");
            }

            return View(user);
        }

        
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string Email = "", string Password = "")
        {
        
            Email ??= "";
            Password ??= "";

            var user = await _mongoService.ValidateLoginAsync(Email, Password);

            if (user != null)
            {
                HttpContext.Session.SetString("UserEmail", user.Email ?? "");
                HttpContext.Session.SetString("UserName", user.FullName ?? "");
                HttpContext.Session.SetString("UserId", user.Id ?? "");

                TempData["Welcome"] = $"Welcome, {user.FullName}";
                return RedirectToAction("Dashboard");
            }

            ViewBag.Error = "❌ Invalid email or password!";
            return View();
        }

        public IActionResult Dashboard()
        {
            var email = HttpContext.Session.GetString("UserEmail");

            if (string.IsNullOrEmpty(email))
                return RedirectToAction("Login");

            ViewBag.UserName = HttpContext.Session.GetString("UserName") ?? "User";

            return View();
        }

      
        [HttpGet]
        public IActionResult BookAppointment()
        {
            var email = HttpContext.Session.GetString("UserEmail");

            if (string.IsNullOrEmpty(email))
                return RedirectToAction("Login");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> BookAppointment(Appointment appointment)
        {
            if (ModelState.IsValid)
            {
                appointment.PatientId = HttpContext.Session.GetString("UserId") ?? "";
                appointment.Status = "Pending";

                await _mongoService.BookAppointmentAsync(appointment);

                TempData["Success"] = "✅ Appointment booked successfully!";
                return RedirectToAction("Dashboard");
            }

            return View(appointment);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["Success"] = "You have logged out successfully!";
            return RedirectToAction("Login");
        }
    }
}

