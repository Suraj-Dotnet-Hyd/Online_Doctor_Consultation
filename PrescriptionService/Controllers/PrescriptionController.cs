using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PrescriptionService.DTOs;
using PrescriptionService.Service;

namespace PrescriptionService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PrescriptionController : ControllerBase
    {
        private readonly IPrescriptionService _service;
        public PrescriptionController(IPrescriptionService service) => _service = service;

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PrescriptionCreateDto dto)
        {
            var result = await _service.CreatePrescriptionAsync(dto, Request);
            return CreatedAtAction(nameof(GetById), new { id = result.PrescriptionId }, result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var res = await _service.GetByIdAsync(id, Request);
            if (res == null) return NotFound();
            return Ok(res);
        }

        [HttpGet("patient/{patientId:int}")]
        public async Task<IActionResult> GetByPatient(int patientId)
        {
            var res = await _service.GetByPatientAsync(patientId, Request);
            return Ok(res);
        }

        [HttpGet("doctor/{doctorId:int}")]
        public async Task<IActionResult> GetByDoctor(int doctorId)
        {
            var res = await _service.GetByDoctorAsync(doctorId, Request);
            return Ok(res);
        }
        //[HttpGet("patient/{patientId}")]
        //public async Task<IActionResult> GetPrescriptionsByPatient(int patientId)
        //{
        //    var prescriptions = await _service.GetByPatientId(patientId);

        //    if (prescriptions == null || !prescriptions.Any())
        //        return NotFound("No prescriptions found for this patient");

        //    return Ok(prescriptions);
        //}


    }
}
