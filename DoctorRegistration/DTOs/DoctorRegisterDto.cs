using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoctorRegistration.DTOs
{
    public class DoctorRegisterDto
    {
      


        [Required, MaxLength(100)]
        public string FirstName { get; set; } = null!;

        [Required, MaxLength(100)]
        public string LastName { get; set; } = null!;

        [Required, MaxLength(150)]
        public string Email { get; set; } = null!;

        // Store hashed password (never plain)
        [Required, MaxLength(300)]
        public string Password { get; set; } = null!;

        [MaxLength(15)]
        public string? PhoneNo { get; set; }

        [Required, MaxLength(150)]
        public string Specialization { get; set; } = null!;

        [Required, MaxLength(50)]
        public string RegistrationNumber { get; set; } = null!;

        [MaxLength(200)]
        public string? HospitalName { get; set; }

        [Range(0, int.MaxValue)]
        public int Experiences { get; set; }

    }
}
