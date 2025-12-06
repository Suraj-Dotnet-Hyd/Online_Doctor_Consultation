using VideoIntegrationService.Models;

namespace VideoIntegrationService.Services
{
    public interface ISessionService
    {
        Task<VideoSession> CreateSessionAsync(VideoSession session);
        Task<VideoSession?> GetByIdAsync(Guid id);
        Task<VideoSession?> GetByAppointmentIdAsync(int appointmentId);
        Task SaveChangesAsync();
    }
}
