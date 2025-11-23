using DoctorAppointmentSystem.Models;
using DoctorAppointmentSystem.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace DoctorAppointmentSystem.Controllers
{
    public class PatientController : Controller
    {
        private readonly MongoDBService _mongoDBService;

        public PatientController(MongoDBService mongoDBService)
        {
            _mongoDBService = mongoDBService;
        }

       
        public async Task<IActionResult> Dashboard()
        {
            var doctors = await _mongoDBService.GetAllDoctorsAsync();
            return View(doctors);
        }

       
        [HttpGet]
        public IActionResult Book(string doctorId)
        {
            ViewBag.DoctorId = doctorId;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Book(Appointment appointment)
        {
            appointment.Status = "Pending";
            appointment.AppointmentDate = DateTime.Now; // 👈 fixed line

            await _mongoDBService.BookAppointmentAsync(appointment);


            ViewBag.Message = "✅ Appointment booked successfully!";
            return View();
        }

        
        public async Task<IActionResult> MyAppointments(string patientId)
        {
            var list = await _mongoDBService.GetAppointmentsByPatientAsync(patientId);
            return View(list);
        }

        public async Task<IActionResult> Cancel(string id)
        {
            await _mongoDBService.UpdateAppointmentStatusAsync(id, "Cancelled");
            return RedirectToAction("MyAppointments");
        }
    }
}

