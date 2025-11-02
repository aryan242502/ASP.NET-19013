using DoctorAppointmentSystem.Models;
using DoctorAppointmentSystem.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DoctorAppointmentSystem.Controllers
{
    public class AppointmentController : Controller
    {
        private readonly DoctorService _doctorService;
        private readonly MongoDBService _mongoService;

        public AppointmentController(DoctorService doctorService, MongoDBService mongoService)
        {
            _doctorService = doctorService;
            _mongoService = mongoService;
        }

        // ✅ GET: Book Appointment Page
        [HttpGet]
        public async Task<IActionResult> BookAppointment(string doctorId)
        {
            if (string.IsNullOrEmpty(doctorId))
                return RedirectToAction("Index", "Home");

            var doctor = _doctorService.GetDoctorById(doctorId);
            if (doctor == null)
                return NotFound("Doctor not found");

            var model = new Appointment
            {
                DoctorId = doctor.Id,
                DoctorName = doctor.Name
            };

            return View(model);
        }

        // ✅ POST: Confirm Appointment
        [HttpPost]
        public async Task<IActionResult> BookAppointment(Appointment appointment)
        {
            if (!ModelState.IsValid)
                return View(appointment);

            // 🧠 Check if this doctor is already booked for the same time slot
            var existingAppointments = await _mongoService.GetAppointmentsByDoctorAsync(appointment.DoctorId);

            bool slotTaken = existingAppointments.Any(a =>
                a.AppointmentDate.Date == appointment.AppointmentDate.Date &&
                a.TimeSlot == appointment.TimeSlot &&
                a.Status != "Cancelled" &&
                a.Status != "Rejected"
            );

            if (slotTaken)
            {
                TempData["Error"] = "⚠️ This doctor is already booked for the selected time!";
                return View(appointment);
            }

            // ✅ Save appointment
            appointment.Status = "Pending";
            await _mongoService.BookAppointmentAsync(appointment);




            TempData["Success"] = "✅ Appointment booked successfully!";
            return RedirectToAction("Index", "Home");
        }
    }
}
