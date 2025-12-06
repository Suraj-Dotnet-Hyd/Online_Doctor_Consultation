using AppointementServiecs.DTOs;
using AppointementServiecs.Model;
using AppointementServiecs.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AppointementServiecs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointementRepository _repository;
        public AppointmentController(IAppointementRepository repository)
        {
            _repository = repository;
        }
        [HttpPost("CreateAppointment")]
        public async Task<ActionResult<Appointment>> CreateAppointment(Appointment appointment)
        {
           await _repository.AddAsync(appointment);
            return Ok(appointment);
        }
        // GET /api/appointments/doctor/12?date=2025-11-28&timezone=Asia/Kolkata
        [HttpGet("doctor/{doctorId}")]
        public async Task<IActionResult> GetDoctorByDate(int doctorId, [FromQuery] DateTime date, [FromQuery] string timezone = "UTC")
        {
            var list = await _repository.GetDoctorAppointmentsByDateAsync(doctorId, date);
            return Ok(list);
        }

        // GET /api/appointments/patient/45?filter=past&page=1&pageSize=20
        [HttpGet("patient/{patientId}")]
        public async Task<IActionResult> GetPatientAppointments(int patientId,[FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var past = await _repository.GetPatientPastAppointmentsAsync(patientId, page, pageSize);
            var future = await _repository.GetPatientFutureAppointmentsAsync(patientId, page, pageSize);
            return Ok(new { past, future });
        }
        [HttpPut("Update/{appointmentId}")]
        public async Task<ActionResult<Appointment>> UpdateAppointmentAsync(int appointmentId, UpdateDto dto)
        {
            var appointment =await _repository.UpdateAppointmentAsync(appointmentId, dto);
            return Ok(appointment);
        }
        [HttpGet("{appointmentId}")]
        public async Task<ActionResult<Appointment>> GetByAppointmentId(int appointmentId)
        {
            return  await _repository.GetAppointmentById(appointmentId);
        }

    }
}
