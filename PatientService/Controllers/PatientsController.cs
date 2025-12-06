using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PatientService.DTOs;
using PatientService.Service;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PatientService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientsController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IPatientService _service;

        public PatientsController(IPatientService service, IConfiguration configuration)
        {
            _service = service;
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PatientCreateDto dto)
        {
            var result = await _service.CreatePatient(dto);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _service.GetPatient(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllPatients();
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] PatientUpdateDto dto)
        {
            var result = await _service.UpdatePatient( id,dto);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeletePatient(id);
            return NoContent();
        }
        [Authorize]          // ONLY this API needs JWT
        [HttpGet("profile")]
        public IActionResult GetProfile()
        {
            return Ok("Patient Profile Data");
        }

        [HttpPost("login")]
        public async Task<IActionResult> PatientLogin([FromBody] PatientLoginDto login)
        {
            // Authenticate patient
            var patient = await _service.LoginPatient(login);

            if (patient == null)
                return Unauthorized("Invalid email or password");

            // JWT claims
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, patient.Email),
        new Claim("PatientId", patient.PatientId.ToString())
    };

            // JWT key
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Create token
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds
            );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            // Return token + patientId
            return Ok(new
            {
                token = jwt,
                patientId = patient.PatientId,
                expires = token.ValidTo
            });
        }


    }
}
