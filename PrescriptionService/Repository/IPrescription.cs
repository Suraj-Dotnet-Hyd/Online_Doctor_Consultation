using PrescriptionService.Models;

namespace PrescriptionService.Repository
{
    public interface IPrescription
    {
        Task<Prescription> CreateAsync(Prescription prescription);
        Task<Prescription> GetByIdAsync(int id);
        Task<IEnumerable<Prescription>> GetByPatientAsync(int patientId);
        Task<IEnumerable<Prescription>> GetByDoctorAsync(int doctorId);
        Task<IEnumerable<Prescription>> GetByPatientIdAsync(int patientId);

    }
}
