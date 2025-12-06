using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoctorRegistration.DTOs
{
    public class DoctorUpdateDto
    {

        [Required, MaxLength(100)]
        public string FirstName { get; set; } = null!;

        [Required, MaxLength(100)]
        public string LastName { get; set; } = null!;

        [Required, MaxLength(150)]
        [EmailAddress]
        public string Email { get; set; } = null!;
        public IFormFile? Image { get; set; }

        // Store hashed password (never plain)
        [Required,MinLength(6), MaxLength(300)]
        public string Password { get; set; } = null!;

        [MaxLength(15)]
        public string? PhoneNo { get; set; }

        [MaxLength(200)]
        public string? HospitalName { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        [Range(0, double.MaxValue)]
        public decimal ConsultationFee { get; set; }
    }
}
