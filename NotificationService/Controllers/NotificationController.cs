using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NotificationService.Models;
using NotificationService.Service;

namespace NotificationService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _service;

        public NotificationController(INotificationService service)
        {
            _service = service;
        }

        [HttpPost("send-email")]
        public async Task<IActionResult> SendEmail([FromBody] NotificationRequest request)
        {
            var result = await _service.SendEmailAsync(request);
            return result ? Ok("Email sent") : BadRequest("Failed to send email");
        }
    }
}
