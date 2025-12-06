namespace PrescriptionService.DTOs
{
    public class PrescriptionResponseDto
    {
        public int PrescriptionId { get; set; }
        public int AppointmentId { get; set; }
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string PdfUrl { get; set; } // full URL to frontend
        public List<PrescriptionItemDto> Items { get; set; } = new();
    }
}
