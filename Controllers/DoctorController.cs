using DoctorAppointmentSystem.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace DoctorAppointmentSystem.Controllers
{
    public class DoctorController : Controller
    {
        private readonly MongoDBService _mongoService;

        public DoctorController(MongoDBService mongoService)
        {
            _mongoService = mongoService;
        }

        // ✅ Dashboard showing doctor's appointments
        public async Task<IActionResult> Dashboard()
        {
            var doctorId = HttpContext.Session.GetString("DoctorId");
            if (string.IsNullOrEmpty(doctorId))
                return RedirectToAction("Login", "User");

            var appointments = await _mongoService.GetAppointmentsByDoctorAsync(doctorId);
            return View(appointments);
        }

        // ✅ Accept Appointment
        public async Task<IActionResult> Accept(string id)
        {
            await _mongoService.UpdateAppointmentStatusAsync(id, "Accepted");
            return RedirectToAction("Dashboard");
        }

        // ✅ Reject Appointment
        public async Task<IActionResult> Reject(string id)
        {
            await _mongoService.UpdateAppointmentStatusAsync(id, "Rejected");
            return RedirectToAction("Dashboard");
        }
    }
}
