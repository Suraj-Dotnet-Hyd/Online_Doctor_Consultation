using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoctorSlots_MicroServices.Models
{
    public class Doctor
    {
        [Key]
        public int DoctorId { get; set; }

        [Required, MaxLength(100)]
        public string FirstName { get; set; } = null!;

        [Required, MaxLength(100)]
        public string LastName { get; set; } = null!;

        // Store relative URL like "/images/DefaultImage.webp" or full URL
        public string? Image { get; set; }

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

        [Column(TypeName = "decimal(10,2)")]
        [Range(0, double.MaxValue)]
        public decimal ConsultationFee { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal AmountEarned { get; set; } = 0m; // default to 0

        // Rating between 0.00 and 5.00 (nullable until reviews)
        [Column(TypeName = "decimal(3,2)")]
        [Range(0.00, 5.00)]
        public decimal? Rating { get; set; }
    }
}
