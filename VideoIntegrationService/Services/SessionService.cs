using Microsoft.EntityFrameworkCore;
using VideoIntegrationService.Data;
using VideoIntegrationService.Models;

namespace VideoIntegrationService.Services
{
    public class SessionService : ISessionService
    {
        private readonly AppDbContext _db;
        public SessionService(AppDbContext db) => _db = db;

        public async Task<VideoSession> CreateSessionAsync(VideoSession session)
        {
            _db.VideoSessions.Add(session);
            await _db.SaveChangesAsync();
            return session;
        }

        public async Task<VideoSession?> GetByIdAsync(Guid id)
        {
            return await _db.VideoSessions.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<VideoSession?> GetByAppointmentIdAsync(int appointmentId)
        {
            return await _db.VideoSessions.FirstOrDefaultAsync(x => x.AppointmentId == appointmentId);
        }

        public async Task SaveChangesAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}
