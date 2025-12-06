using System.ComponentModel.DataAnnotations;

namespace PrescriptionService.Models
{
    public class Prescription
    {
        [Key]
        public int PrescriptionId { get; set; }

        [Required]
        public int AppointmentId { get; set; }

        [Required]
        public int PatientId { get; set; }

        [Required]
        public int DoctorId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // relative path like "prescriptions/prescription_123.pdf"
        [MaxLength(500)]
        public string PdfPath { get; set; }

        public ICollection<PrescriptionItem> Items { get; set; } = new List<PrescriptionItem>();
    }
}
