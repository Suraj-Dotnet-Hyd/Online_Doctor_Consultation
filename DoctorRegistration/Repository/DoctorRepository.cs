using DoctorRegistration.DTOs;
using DoctorRegistration.Model;
using Microsoft.EntityFrameworkCore;

namespace DoctorRegistration.Repository
{
    public class DoctorRepository : IDoctorRepository
    {
        private AppDbContext _context;
        public DoctorRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Doctor>> FilterDoctorsAsync(DoctorFilterRequest filter)
        {
            var query = _context.Doctors.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.Specialization))
            {
                query = query.Where(d => d.Specialization == filter.Specialization);
            }

            if (filter.MinExperience.HasValue)
            {
                query = query.Where(d => d.Experiences >= filter.MinExperience.Value);
            }

            if (filter.MinRating.HasValue)
            {
                query = query.Where(d => d.Rating >= filter.MinRating.Value);
            }

            if (filter.MaxConsultationFee.HasValue)
            {
                query = query.Where(d => d.ConsultationFee <= filter.MaxConsultationFee.Value);
            }

            // Executes SQL only here
            return await query.ToListAsync();
        }

        public async Task<IEnumerable<Doctor>> GetAll()
        {
            var doc = await _context.Doctors.ToListAsync();
            return (doc);
        }

        public async Task<Doctor> GetDoctorByIdAsync(int id)
        {
            var doc = await _context.Doctors.FirstOrDefaultAsync(x => x.DoctorId == id);
            return (doc);
        }

        public async Task<Doctor> LoginDoctor(LoginDTO loginDTO)
        {
            var dr = await _context.Doctors.FirstOrDefaultAsync(x => x.Email == loginDTO.Email);

            if (dr == null)
                return null;

            if (dr.Password != loginDTO.Password)
                return null;

            return dr; // return doctor object
        }



        //public async Task RegisterDoctor(Doctor doctor)
        //{
        //    _context.Doctors.Add(doctor);
        //    await _context.SaveChangesAsync();
        //}

        public async Task<DoctorRegisterDto> RegisterDoctor(DoctorRegisterDto registerDTO)
        {
            Doctor doc = new Doctor() { Email = registerDTO.Email, Experiences=registerDTO.Experiences, FirstName=registerDTO.FirstName, LastName=registerDTO.LastName, PhoneNo=registerDTO.PhoneNo, HospitalName=registerDTO.HospitalName, Password=registerDTO.Password, Specialization=registerDTO.Specialization, RegistrationNumber=registerDTO.RegistrationNumber};
             await _context.Doctors.AddAsync(doc);
            await _context.SaveChangesAsync();
            return registerDTO;

        }
        //public async Task<DoctorUpdateDto> UpdateDoctor(DoctorUpdateDto updateDTO)
        //{
        //    var doc = _context.Doctors.FirstOrDefault(x=>x.Email == updateDTO.Email);   
        //    if (doc != null)
        //    {
        //        doc.FirstName=updateDTO.FirstName;
        //        doc.LastName=updateDTO.LastName;
        //        doc.PhoneNo=updateDTO.PhoneNo;
        //        doc.ConsultationFee=updateDTO.ConsultationFee;
        //        doc.HospitalName=updateDTO.HospitalName;
        //        doc.Password=updateDTO.Password;
        //        doc.Email = updateDTO.Email;
        //        doc.Image = updateDTO.Image;
        //    }
        //    return updateDTO;
        //}
        public async Task<DoctorUpdateDto> UpdateDoctor(DoctorUpdateDto updateDTO)
        {
            var doc = await _context.Doctors.FirstOrDefaultAsync(x => x.Email == updateDTO.Email);

            if (doc == null)
                return null;

            // Update fields
            doc.FirstName = updateDTO.FirstName;
            doc.LastName = updateDTO.LastName;
            doc.PhoneNo = updateDTO.PhoneNo;
            doc.ConsultationFee = updateDTO.ConsultationFee;
            doc.HospitalName = updateDTO.HospitalName;
            doc.Password = updateDTO.Password;

            // ------ FILE UPLOAD HANDLING ------
            if (updateDTO.Image != null && updateDTO.Image.Length > 0)
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/profilepics");

                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                string uniqueFileName = $"doctor_{doc.DoctorId}_{Guid.NewGuid()}{Path.GetExtension(updateDTO.Image.FileName)}";
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await updateDTO.Image.CopyToAsync(stream);
                }

                // Save only relative path to DB
                doc.Image = $"images/profilepics/{uniqueFileName}";
            }

            await _context.SaveChangesAsync();

            // Return updated details + new image path
            return new DoctorUpdateDto
            {
                FirstName = doc.FirstName,
                LastName = doc.LastName,
                Email = doc.Email,
                PhoneNo = doc.PhoneNo,
                ConsultationFee = doc.ConsultationFee,
                HospitalName = doc.HospitalName,
                Password = doc.Password,
                // NEW: full URL for frontend
                Image = null
            };
        }

    }
}
