using System.ComponentModel.DataAnnotations;

namespace VideoIntegrationService.Models
{
    public class ChatMessage
    {
        [Key]
        public Guid Id { get; set; }
        public Guid SessionId { get; set; }   // VideoSession.Id
        public int SenderId { get; set; }     // doctorId or patientId
        public string SenderName { get; set; } = null!;
        public string Message { get; set; } = null!;
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
    }
}
