using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PrescriptionService.Models
{
    public class PrescriptionItem
    {
        [Key]
        public int PrescriptionItemId { get; set; }

        [Required]
        public int PrescriptionId { get; set; }

        [Required, MaxLength(250)]
        public string Medicine { get; set; }

        [MaxLength(100)]
        public string Dosage { get; set; }

        [MaxLength(100)]
        public string Duration { get; set; }

        [MaxLength(1000)]
        public string Notes { get; set; }

        [ForeignKey(nameof(PrescriptionId))]
        public Prescription Prescription { get; set; }
    }
}
