using Microsoft.EntityFrameworkCore;
using PatientService.Data;
using PatientService.DTOs;
using PatientService.Models;

namespace PatientService.Repositories
{
    public class PatientRepositoy : IPatientRepository
    {
        private readonly AppDbContext _context;
        public PatientRepositoy(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Patient> GetByIdAsync(int id)
        {
            return await _context.patients.FirstOrDefaultAsync(x => x.PatientId == id);
        }

        public async Task<IEnumerable<Patient>> GetAllAsync()
        {

            return await _context.patients.ToListAsync();
        }
        //public async Task<PatientCreateDto> CreateAsync(PatientCreateDto patient)
        //{
        //    var pat = await _context.patients.FirstOrDefaultAsync(x => x.Email == patient.Email);
        //    if (pat != null)
        //    {
        //        pat.Email = patient.Email;
        //        pat.FirstName = patient.FirstName;
        //        pat.LastName = patient.LastName;
        //        pat.BloodGroup = patient.BloodGroup;
        //        pat.UpdatedAt = DateTime.Now;
        //        pat.DateOfBirth = DateTime.Now;
        //        pat.Phone = patient.Phone;
        //        pat.Allergies = patient.Allergies;
        //        pat.MedicalHistory = patient.MedicalHistory;
        //        pat.PassHash = patient.PassHash;
        //        //await _context.SaveChangesAsync();


        //    }

        //    await _context.patients.AddAsync(pat);
        //    await _context.SaveChangesAsync();
        //    return patient;
        //}
        public async Task<PatientCreateDto> CreateAsync(PatientCreateDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            var existing = await _context.patients.FirstOrDefaultAsync(x => x.Email == dto.Email);

            if (existing != null)
                throw new Exception("Patient already exists with this email");

            var patient = new Patient
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Phone = dto.Phone,
                BloodGroup = dto.BloodGroup,
                Allergies = dto.Allergies,
                MedicalHistory = dto.MedicalHistory,
                DateOfBirth = dto.DateOfBirth,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                PassHash = dto.PassHash
               
            };

            await _context.patients.AddAsync(patient);
            await _context.SaveChangesAsync();

            return dto;
        }

        public async Task<PatientUpdateDto> UpdateAsync( int Id,PatientUpdateDto dto)
        {
            var patient = await _context.patients.FirstOrDefaultAsync(x => x.PatientId==Id);
            if (patient == null)
                return null;

            patient.FirstName = dto.FirstName;
            patient.LastName = dto.LastName;
            patient.Email = dto.Email;
            //patient.Image = dto.Image;
            patient.Phone = dto.Phone;
            patient.BloodGroup = dto.BloodGroup;
            patient.Allergies = dto.Allergies;
            patient.MedicalHistory = dto.MedicalHistory;
            patient.PassHash = dto.PassHash;
            patient.UpdatedAt = DateTime.Now;

            // ---- FILE UPLOAD ----
            if (dto.Image != null && dto.Image.Length > 0)
            {
                string folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/profilepics");
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                string fileName = $"patient_{patient.FirstName}_{Guid.NewGuid()}{Path.GetExtension(dto.Image.FileName)}";
                string filePath = Path.Combine(folder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.Image.CopyToAsync(stream);
                }

                patient.Image = $"images/profilepics/{fileName}";
            }

            await _context.SaveChangesAsync();
            return dto;
        }

        public async Task DeleteAsync(int id)
        {
            var pat = await GetByIdAsync(id);
            if (pat != null)
            {
                _context.patients.Remove(pat);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Patient> LoginPatient(PatientLoginDto patientLoginDto)
        {
            var pat = await _context.patients.FirstOrDefaultAsync(x => x.Email == patientLoginDto.Email);
           
            return pat;
        }
    }
}
