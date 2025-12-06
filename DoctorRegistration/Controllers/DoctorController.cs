using DoctorRegistration.DTOs;
using DoctorRegistration.Model;
using DoctorRegistration.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.WebSockets;
using System.Numerics;
using System.Security.Claims;
using System.Text;


namespace DoctorRegistration.Controllers
{
    [ApiController]
    [Route("api/doctors")]
    public class DoctorController : ControllerBase
    {
        private readonly IDoctorRepository _repo;
        private readonly IConfiguration _configuration;


        public DoctorController(IDoctorRepository repo, IConfiguration configuration)
        {
            _repo = repo;
            _configuration = configuration;   // store configuration here
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<Doctor>>> GetAll()
        {
            var res = await _repo.GetAll();
            return Ok(res);
        }

        [HttpGet("filter")]
        public async Task<IActionResult> FilterDoctors([FromQuery] DoctorFilterRequest filter)
        {
            var doctors = await _repo.FilterDoctorsAsync(filter);
            return Ok(doctors);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetDoctorById(int id)
        {
            var doctor = await _repo.GetDoctorByIdAsync(id);
            if (doctor == null) return NotFound();
            return Ok(doctor);
        }

        [HttpPost]
        public async Task<ActionResult<DoctorRegisterDto>> RegisterDoctor([FromBody] DoctorRegisterDto doctor)
        {
            await _repo.RegisterDoctor(doctor);
            return Ok(doctor);
        }
        [Authorize]          // ONLY this API needs JWT
        [HttpGet("profile")]
        public IActionResult GetProfile()
        {
            return Ok("Doctor Profile Data");
        }


        [HttpPost("login")]
        public async Task<ActionResult> DoctorLogin([FromBody] LoginDTO login)
        {
            var doctor = await _repo.LoginDoctor(login);

            if (doctor == null)
                return Unauthorized("Invalid email or password");

            // JWT claims
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, doctor.Email),
        new Claim("DoctorId", doctor.DoctorId.ToString())
    };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds
            );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new
            {
                token = jwt,
                expires = token.ValidTo,
                doctorId = doctor.DoctorId   // 🔥 Added doctorId
            });
        }


        //[HttpPut("Update")]
        //public async Task<ActionResult<DoctorUpdateDto>> DoctorUpdate(DoctorUpdateDto doctorUpdate)
        //{
        //    var updatedDoc = await _repo.UpdateDoctor(doctorUpdate);
        //    return Ok(updatedDoc);
        //}
        [HttpPut("update")]
        public async Task<ActionResult> DoctorUpdate([FromForm] DoctorUpdateDto dto)
        {
            var updated = await _repo.UpdateDoctor(dto);
            if (updated == null)
                return NotFound();

            return Ok(updated);
        }


    }

}
