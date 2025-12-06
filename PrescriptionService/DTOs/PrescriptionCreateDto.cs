namespace PrescriptionService.DTOs
{
    public class PrescriptionCreateDto
    {
        public int AppointmentId { get; set; }
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public List<PrescriptionItemDto> Items { get; set; } = new();
        public string Notes { get; set; } // optional overall notes
    }
}
