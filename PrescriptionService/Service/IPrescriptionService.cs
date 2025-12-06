using PrescriptionService.DTOs;

namespace PrescriptionService.Service
{
    public interface IPrescriptionService
    {
        Task<PrescriptionResponseDto> CreatePrescriptionAsync(PrescriptionCreateDto dto, HttpRequest request);
        Task<PrescriptionResponseDto> GetByIdAsync(int id, HttpRequest request);
        Task<IEnumerable<PrescriptionResponseDto>> GetByPatientAsync(int patientId, HttpRequest request);
        Task<IEnumerable<PrescriptionResponseDto>> GetByDoctorAsync(int doctorId, HttpRequest request);
        Task<IEnumerable<PrescriptionResponseDto>> GetByPatientId(int patientId);

    }
}
