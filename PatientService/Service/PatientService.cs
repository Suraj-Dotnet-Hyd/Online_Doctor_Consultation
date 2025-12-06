using Microsoft.EntityFrameworkCore;
using PatientService.DTOs;
using PatientService.Models;
using PatientService.Repositories;

namespace PatientService.Service
{
    public class PatientService : IPatientService
    {
        private readonly IPatientRepository _repo;

        public PatientService(IPatientRepository repo)
        {
            _repo = repo;
        }

        public async Task<PatientCreateDto> CreatePatient(PatientCreateDto dto)
        {
            var patient = new PatientCreateDto
            {
               
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Phone = dto.Phone,
                Email = dto.Email,
                PassHash = dto.PassHash,
                DateOfBirth = dto.DateOfBirth,
                MedicalHistory = dto.MedicalHistory,
                Allergies = dto.Allergies,
                BloodGroup = dto.BloodGroup,
                //InsuranceProvider = dto.InsuranceProvider,
                //InsuranceNumber = dto.InsuranceNumber
            };

            var created = await _repo.CreateAsync(patient);

            return created;
        }

        public async Task DeletePatient(int id)
        {
            await _repo.DeleteAsync(id);
        }

        public async Task<IEnumerable<PatientResponseDto>> GetAllPatients()
        {
            var list = await _repo.GetAllAsync();
            return list.Select(ToDto);
        }

        public async Task<PatientResponseDto> GetPatient(int id)
        {
            var patient = await _repo.GetByIdAsync(id);
            return patient == null ? null : ToDto(patient);
        }

        //public async Task<PatientUpdateDto> UpdatePatient(int id, PatientUpdateDto dto)
        //{
        //    var patient = await _repo.GetByIdAsync(id);
        //    if (patient == null) return null;

        //    patient.Phone = dto.Phone;
        //    patient.Email = dto.Email;
        //    patient.FirstName = dto.FirstName;
        //    patient.LastName = dto.LastName;
        //    patient.PassHash = dto.PassHash;
        //    patient.Image = null;
        //    patient.MedicalHistory = dto.MedicalHistory;
        //    patient.Allergies = dto.Allergies;
        //    patient.BloodGroup = dto.BloodGroup;
        //    //patient.InsuranceProvider = dto.InsuranceProvider;
        //    //patient.InsuranceNumber = dto.InsuranceNumber;

        //    // ------ FILE UPLOAD HANDLING ------
        //    if (dto.Image != null && dto.Image.Length > 0)
        //    {
        //        string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/profilepics");

        //        if (!Directory.Exists(uploadsFolder))
        //            Directory.CreateDirectory(uploadsFolder);

        //        string uniqueFileName = $"doctor_{patient.PatientId}_{Guid.NewGuid()}{Path.GetExtension(dto.Image.FileName)}";
        //        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

        //        using (var stream = new FileStream(filePath, FileMode.Create))
        //        {
        //            await dto.Image.CopyToAsync(stream);
        //        }

        //        // Save only relative path to DB
        //        patient.Image = $"images/profilepics/{uniqueFileName}";
        //    }

        //    //await _context.SaveChangesAsync();

        //    // Return updated details + new image path
        //    return new PatientUpdateDto
        //    {
        //        FirstName = dto.FirstName,
        //        LastName = dto.LastName,
        //        Email = dto.Email,

        //        Phone = dto.Phone,
        //        MedicalHistory= dto.MedicalHistory,
        //        Allergies = dto.Allergies,
        //        BloodGroup = dto.BloodGroup,

        //        PassHash = dto.PassHash,
        //        // NEW: full URL for frontend
        //        Image = null
        //    };


        //    return await _repo.UpdateAsync(dto);
        //}

        public async Task<PatientUpdateDto> UpdatePatient(int Id, PatientUpdateDto dto)
        {
            var updated = await _repo.UpdateAsync( Id,dto);
            if (updated == null) return null;

            return dto;
        }


        private PatientResponseDto ToDto(Patient p)
        {
            return new PatientResponseDto
            {
                PatientId = p.PatientId,
                
               
                FirstName = p.FirstName,
                LastName = p.LastName,
                Phone = p.Phone,
                Email=p.Email,
                PassHash=p.PassHash,
                Image= p.Image,
                DateOfBirth = p.DateOfBirth,
                MedicalHistory = p.MedicalHistory,
                Allergies = p.Allergies,
                BloodGroup = p.BloodGroup,
                //InsuranceProvider = p.InsuranceProvider,
                //InsuranceNumber = p.InsuranceNumber
            };
        }
        public async Task<Patient> LoginPatient(PatientLoginDto loginDto)
        {
            return await _repo.LoginPatient(loginDto);
        }
    }
}
