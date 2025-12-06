using VideoIntegrationService.Data;

namespace VideoIntegrationService.Services
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _env;
        private readonly AppDbContext _db;

        public FileService(IWebHostEnvironment env, AppDbContext db)
        {
            _env = env;
            _db = db;
        }

        public async Task<string> SaveRecordingAsync(Guid sessionId, IFormFile file)
        {
            var uploadsRoot = Path.Combine(_env.ContentRootPath, "wwwroot", "uploads", "recordings");
            Directory.CreateDirectory(uploadsRoot);

            var fileName = $"{sessionId}_{DateTime.UtcNow:yyyyMMddHHmmss}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploadsRoot, fileName);

            await using (var fs = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fs);
            }

            var url = $"/uploads/recordings/{fileName}";

            var session = await _db.VideoSessions.FindAsync(sessionId);
            if (session != null)
            {
                session.RecordingUrl = url;
                session.UpdatedAt = DateTime.UtcNow;
                await _db.SaveChangesAsync();
            }

            return url;
        }

        public async Task<string> SavePrescriptionAsync(Guid sessionId, IFormFile file)
        {
            var uploadsRoot = Path.Combine(_env.ContentRootPath, "wwwroot", "uploads", "prescriptions");
            Directory.CreateDirectory(uploadsRoot);

            var fileName = $"{sessionId}_{DateTime.UtcNow:yyyyMMddHHmmss}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploadsRoot, fileName);

            await using (var fs = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fs);
            }

            var url = $"/uploads/prescriptions/{fileName}";
            return url;
        }
    }
}
