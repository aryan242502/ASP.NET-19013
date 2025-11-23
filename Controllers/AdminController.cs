using DoctorAppointmentSystem.Models;
using DoctorAppointmentSystem.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DoctorAppointmentSystem.Controllers
{
    public class AdminController : Controller
    {
        private readonly MongoDBService _mongoService;

        public AdminController(MongoDBService mongoService)
        {
            _mongoService = mongoService;
        }

        public async Task<IActionResult> Dashboard()
        {
            ViewBag.DoctorCount = (await _mongoService.GetAllDoctorsAsync()).Count;
            ViewBag.UserCount = (await _mongoService.GetAllUsersAsync()).Count;
            ViewBag.AppointmentCount = (await _mongoService.GetAllAppointmentsAsync()).Count;
            return View();
        }

        
        public async Task<IActionResult> ManageDoctors()
        {
            var doctors = await _mongoService.GetAllDoctorsAsync();
            return View(doctors);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteDoctor(string id)
        {
            if (!string.IsNullOrEmpty(id))
                await _mongoService.DeleteDoctorAsync(id);

            TempData["Message"] = "Doctor deleted successfully.";
            return RedirectToAction("ManageDoctors");
        }

        public async Task<IActionResult> ManageUsers()
        {
            var users = await _mongoService.GetAllUsersAsync();
            return View(users);
        }

        
        [HttpGet]
        public async Task<IActionResult> ManageAppointments()
        {
            var appointments = await _mongoService.GetAllAppointmentsAsync();
            return View(appointments);
        }

        [HttpPost]
        public async Task<IActionResult> AcceptAppointment(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                await _mongoService.UpdateAppointmentStatusAsync(id, "Accepted");
                TempData["Message"] = "✅ Appointment accepted successfully!";
            }
            return RedirectToAction("ManageAppointments");
        }

        [HttpPost]
        public async Task<IActionResult> RejectAppointment(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                await _mongoService.UpdateAppointmentStatusAsync(id, "Rejected");
                TempData["Message"] = "❌ Appointment rejected successfully!";
            }
            return RedirectToAction("ManageAppointments");
        }
    }
}

