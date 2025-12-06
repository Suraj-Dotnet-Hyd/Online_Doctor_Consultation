namespace VideoIntegrationService.Services
{
    public interface IFileService
    {
        Task<string> SaveRecordingAsync(Guid sessionId, IFormFile file);
        Task<string> SavePrescriptionAsync(Guid sessionId, IFormFile file);
    }
}
