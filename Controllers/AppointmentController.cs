using DoctorAppointmentSystem.Models;
using DoctorAppointmentSystem.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Linq;

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

        // ✅ BOOK APPOINTMENT (GET)
        [HttpGet]
        public IActionResult BookAppointment(string doctorId)
        {
            if (string.IsNullOrEmpty(doctorId))
                return RedirectToAction("Index", "Home");

            var doctor = _doctorService.GetDoctorById(doctorId);
            if (doctor == null)
                return NotFound("Doctor not found");

            return View(new Appointment
            {
                DoctorId = doctor.Id,
                DoctorName = doctor.Name
            });
        }

        // ✅ BOOK APPOINTMENT (POST)
        [HttpPost]
        public async Task<IActionResult> BookAppointment(Appointment appointment)
        {
            if (!ModelState.IsValid)
                return View(appointment);

            var slotTaken = (await _mongoService.GetAppointmentsByDoctorAsync(appointment.DoctorId))
                .Any(a =>
                    a.AppointmentDate.Date == appointment.AppointmentDate.Date &&
                    a.TimeSlot == appointment.TimeSlot &&
                    a.Status != "Cancelled" &&
                    a.Status != "Rejected"
                );

            if (slotTaken)
            {
                TempData["Error"] = "⚠️ This time slot is already booked!";
                return View(appointment);
            }

            // ✅ Set Patient Data from Session
            appointment.PatientId = HttpContext.Session.GetString("UserId") ?? "";
            appointment.PatientName = HttpContext.Session.GetString("UserName") ?? "";
            appointment.PatientEmail = HttpContext.Session.GetString("UserEmail") ?? "";
            appointment.Status = "Pending";

            await _mongoService.BookAppointmentAsync(appointment);

            return RedirectToAction("Success");
        }

        public IActionResult Success() => View();

        // ✅ ✅ SHOW ONLY LOGGED-IN USER APPOINTMENTS
        public async Task<IActionResult> MyAppointments()
        {
            var patientId = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(patientId))
                return RedirectToAction("Login", "Account");

            var appointments = await _mongoService.GetAppointmentsByPatientAsync(patientId);

            return View(appointments.OrderByDescending(a => a.AppointmentDate));
        }

        // ✅ DETAILS
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id)) return BadRequest();

            var appt = await _mongoService.GetAppointmentByIdAsync(id);
            if (appt == null) return NotFound();

            return View(appt);
        }

        // ✅ EDIT GET
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id)) return BadRequest();

            var appt = await _mongoService.GetAppointmentByIdAsync(id);
            if (appt == null) return NotFound();

            return View(appt);
        }

        // ✅ EDIT POST
        [HttpPost]
        public async Task<IActionResult> Edit(string id, Appointment updated)
        {
            if (!ModelState.IsValid)
                return View(updated);

            var existing = await _mongoService.GetAppointmentByIdAsync(id);
            if (existing == null)
                return NotFound();

            existing.AppointmentDate = updated.AppointmentDate;
            existing.TimeSlot = updated.TimeSlot;
            existing.Description = updated.Description;

            await _mongoService.UpdateAppointmentAsync(id, existing);

            TempData["Message"] = "✅ Appointment updated successfully!";
            return RedirectToAction("MyAppointments");
        }

        // ✅ CANCEL APPOINTMENT
        [HttpPost]
        public async Task<IActionResult> Cancel(string id)
        {
            if (string.IsNullOrEmpty(id)) return BadRequest();

            await _mongoService.UpdateAppointmentStatusAsync(id, "Cancelled");
            TempData["Message"] = "✅ Appointment cancelled.";

            return RedirectToAction("MyAppointments");
        }

        // ✅ DELETE 
        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id)) return BadRequest();

            await _mongoService.DeleteAppointmentAsync(id);
            TempData["Message"] = "✅ Appointment deleted.";

            return RedirectToAction("MyAppointments");
        }
    }
}
