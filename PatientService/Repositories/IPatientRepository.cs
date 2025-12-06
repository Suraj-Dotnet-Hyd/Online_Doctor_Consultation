using PatientService.DTOs;
using PatientService.Models;

namespace PatientService.Repositories
{
    public interface IPatientRepository
    {
        Task<Patient> GetByIdAsync(int id);
        Task<IEnumerable<Patient>> GetAllAsync();
        Task<PatientCreateDto> CreateAsync(PatientCreateDto patient);
        Task<PatientUpdateDto> UpdateAsync(int Id,PatientUpdateDto patient);
        Task DeleteAsync(int id);
        Task<Patient> LoginPatient(PatientLoginDto patientLoginDto);
        //Task<PatientCreateDto> CreatePatient(PatientCreateDto patient);
    }
}
