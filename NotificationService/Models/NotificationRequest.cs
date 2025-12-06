namespace NotificationService.Models
{
    public class NotificationRequest
    {
        
            public string ToEmail { get; set; }
            public string ToPhone { get; set; } 
            public string Subject { get; set; }
            public string Message { get; set; }
    }

    
}
