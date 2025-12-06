namespace VideoIntegrationService.Models
{
    public class VideoSession
    {
        public Guid Id { get; set; }               // Internal unique ID  
        public int AppointmentId { get; set; }     // From Appointment Service

        public string RoomName { get; set; } = null!;

        public int DoctorId { get; set; }          // Host
        public int PatientId { get; set; }         // Guest

        public DateTime ScheduledStart { get; set; }
        public DateTime? ScheduledEnd { get; set; }

        public string Status { get; set; } = "Scheduled";

        public string? RecordingUrl { get; set; }
        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
