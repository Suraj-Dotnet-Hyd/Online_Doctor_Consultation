using Microsoft.EntityFrameworkCore;
using PrescriptionService.Data;
using PrescriptionService.Models;

namespace PrescriptionService.Repository
{
    public class PrescriptionRepo : IPrescription
    {
        private readonly AppDbContext _db;
        public PrescriptionRepo(AppDbContext db) => _db = db;

        public async Task<Prescription> CreateAsync(Prescription prescription)
        {
            await _db.Prescriptions.AddAsync(prescription);
            await _db.SaveChangesAsync();
            return prescription;
        }

        public async Task<Prescription> GetByIdAsync(int id)
        {
            return await _db.Prescriptions
                .Include(p => p.Items)
                .FirstOrDefaultAsync(p => p.PrescriptionId == id);
        }

        public async Task<IEnumerable<Prescription>> GetByPatientAsync(int patientId)
        {
            return await _db.Prescriptions
                .Include(p => p.Items)
                .Where(p => p.PatientId == patientId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Prescription>> GetByDoctorAsync(int doctorId)
        {
            return await _db.Prescriptions
                .Include(p => p.Items)
                .Where(p => p.DoctorId == doctorId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }
        public async Task<IEnumerable<Prescription>> GetByPatientIdAsync(int patientId)
        {
            return await _db.Prescriptions
                .Where(x => x.PatientId == patientId)
                .ToListAsync();
        }

    }
}
