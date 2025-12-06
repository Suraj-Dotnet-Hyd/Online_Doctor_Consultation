using PatientService.DTOs;
using PatientService.Models;

namespace PatientService.Service
{
    public interface IPatientService
    {
        Task<PatientCreateDto> CreatePatient(PatientCreateDto dto);
        Task<PatientResponseDto> GetPatient(int id);
        Task<IEnumerable<PatientResponseDto>> GetAllPatients();
        Task<PatientUpdateDto> UpdatePatient(int Id, PatientUpdateDto dto);
        Task DeletePatient(int id);
        Task<Patient> LoginPatient(PatientLoginDto loginDto);
    }
}
