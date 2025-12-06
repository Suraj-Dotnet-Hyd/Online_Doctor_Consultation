using DoctorRegistration.DTOs;
using DoctorRegistration.Model;

namespace DoctorRegistration.Repository
{
    public interface IDoctorRepository
    {
        //public Task<DoctorRegisterDto> Register(DoctorRegisterDto dto);
        public Task<IEnumerable<Doctor>> GetAll();
        public Task<IEnumerable<Doctor>> FilterDoctorsAsync(DoctorFilterRequest filter);
        public Task<Doctor> GetDoctorByIdAsync(int id);
        public Task<DoctorRegisterDto> RegisterDoctor(DoctorRegisterDto doctor);

        public Task<Doctor> LoginDoctor(LoginDTO loginDTO);
        public Task<DoctorUpdateDto> UpdateDoctor(DoctorUpdateDto updateDTO);

    }
}
