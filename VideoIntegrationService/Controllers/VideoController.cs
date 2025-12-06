using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VideoIntegrationService.Data;
using VideoIntegrationService.DTO;
using VideoIntegrationService.Models;
using VideoIntegrationService.Services;

namespace VideoIntegrationService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VideoController : ControllerBase
    {
        private readonly ISessionService _sessionService;
        private readonly IFileService _fileService;
        private readonly ILogger<VideoController> _logger;
        private readonly AppDbContext _db;

        public VideoController(ISessionService sessionService, IFileService fileService, ILogger<VideoController> logger, AppDbContext db   )
        {
            _sessionService = sessionService;
            _fileService = fileService;
            _logger = logger;
            _db = db;
        }

        [HttpPost("create-session")]
        public async Task<IActionResult> CreateSession([FromBody] CreateSessionDto dto)
        {
            // If a session for this appointment already exists, return it
            var existing = await _sessionService.GetByAppointmentIdAsync(dto.AppointmentId);
            if (existing != null)
            {
                return Ok(new
                {
                    sessionId = existing.Id,
                    roomName = existing.RoomName,
                    joinUrl = $"{dto.JitsiBaseUrl}/{existing.RoomName}"
                });
            }

            var roomName = $"appt-{dto.AppointmentId}-{Guid.NewGuid().ToString("N").Substring(0, 6)}";

            var session = new VideoSession
            {
                Id = Guid.NewGuid(),
                AppointmentId = dto.AppointmentId,
                DoctorId = dto.DoctorId,
                PatientId = dto.PatientId,
                RoomName = roomName,
                ScheduledStart = dto.ScheduledStart,
                Status = "Scheduled",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var created = await _sessionService.CreateSessionAsync(session);

            return Ok(new
            {
                sessionId = created.Id,
                roomName = created.RoomName,
                joinUrl = $"{dto.JitsiBaseUrl}/{created.RoomName}"
            });
        }

        [HttpGet("{sessionId:guid}")]
        public async Task<IActionResult> GetSession([FromRoute] Guid sessionId)
        {
            var s = await _sessionService.GetByIdAsync(sessionId);
            if (s == null) return NotFound();
            return Ok(s);
        }

        [HttpPost("{sessionId}/upload-recording")]
        [DisableRequestSizeLimit]
        [RequestFormLimits(MultipartBodyLengthLimit = long.MaxValue)]
        public async Task<IActionResult> UploadRecording(Guid sessionId, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded");

            var folder = Path.Combine(Directory.GetCurrentDirectory(), "Recordings");
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            var fileName = $"recording_{sessionId}_{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(folder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return Ok(new
            {
                message = "Recording uploaded successfully",
                fileUrl = $"https://localhost:7128/Recordings/{fileName}"
            });
        }


        [HttpPost("{sessionId}/upload-prescription")]
        [DisableRequestSizeLimit]
        [RequestFormLimits(MultipartBodyLengthLimit = long.MaxValue)]
        public async Task<IActionResult> UploadPrescription(Guid sessionId, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded");

            var folder = Path.Combine(Directory.GetCurrentDirectory(), "Prescriptions");
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            var fileName = $"prescription_{sessionId}_{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(folder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return Ok(new
            {
                message = "Prescription uploaded successfully",
                fileUrl = $"https://localhost:7128/Prescriptions/{fileName}"
            });
        }


        [HttpPost("{sessionId:guid}/mark-started")]
        public async Task<IActionResult> MarkStarted([FromRoute] Guid sessionId)
        {
            var s = await _sessionService.GetByIdAsync(sessionId);
            if (s == null) return NotFound();
            s.Status = "Started";
            s.UpdatedAt = DateTime.UtcNow;
            await _sessionService.SaveChangesAsync();
            return Ok(s);
        }

        [HttpPost("{sessionId:guid}/mark-ended")]
        public async Task<IActionResult> MarkEnded([FromRoute] Guid sessionId)
        {
            var s = await _sessionService.GetByIdAsync(sessionId);
            if (s == null) return NotFound();
            s.Status = "Ended";
            s.ScheduledEnd = DateTime.UtcNow;
            s.UpdatedAt = DateTime.UtcNow;
            await _sessionService.SaveChangesAsync();
            return Ok(s);
        }
        [HttpGet("by-appointment/{appointmentId}")]
        public async Task<IActionResult> GetByAppointmentId(int appointmentId)
        {
            var session = await _db.VideoSessions
                .FirstOrDefaultAsync(x => x.AppointmentId == appointmentId);

            if (session == null)
                return NotFound();

            return Ok(session);
        }

    }
}
