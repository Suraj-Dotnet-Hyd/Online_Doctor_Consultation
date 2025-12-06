using AppointementServiecs.DTOs;
using AppointementServiecs.Model;
using Microsoft.EntityFrameworkCore;

namespace AppointementServiecs.Repository
{
    // Repositories/AppointmentRepository.cs
    public class AppointmentRepository : IAppointementRepository
    {
        private readonly AppDbContext _db;
        public AppointmentRepository(AppDbContext db) => _db = db;

        public async Task AddAsync(Appointment appointment)
        {
            if (appointment.AppointmentDateTime.Kind == DateTimeKind.Unspecified)
                appointment.AppointmentDateTime = DateTime.SpecifyKind(appointment.AppointmentDateTime, DateTimeKind.Utc);
            else
                appointment.AppointmentDateTime = appointment.AppointmentDateTime.ToUniversalTime();

            await _db.Appointments.AddAsync(appointment);
            await SaveChangesAsync();
        }

        public async Task SaveChangesAsync() => await _db.SaveChangesAsync();

        public async Task<List<Appointment>> GetDoctorAppointmentsByDateAsync(int doctorId, DateTime dateLocal)
{
    // Take only the date part (00:00:00)
    var start = dateLocal.Date;
    var end = start.AddDays(1);

    return await _db.Appointments
        .Where(a =>
            a.DoctorId == doctorId &&
            a.AppointmentDateTime >= start &&
            a.AppointmentDateTime < end &&
            a.Status != "Cancelled")
        .OrderBy(a => a.AppointmentDateTime)
        .ToListAsync();
}

        public async Task<List<Appointment>> GetPatientPastAppointmentsAsync(int patientId, int page = 1, int pageSize = 20)
        {
            var nowUtc = DateTime.UtcNow;
            page = Math.Max(1, page);
            pageSize = Math.Clamp(pageSize, 1, 200);

            return await _db.Appointments
                .Where(a => a.PatientId == patientId && a.AppointmentDateTime < nowUtc)
                .OrderByDescending(a => a.AppointmentDateTime)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<List<Appointment>> GetPatientFutureAppointmentsAsync(int patientId, int page = 1, int pageSize = 20)
        {
            var nowUtc = DateTime.UtcNow;
            page = Math.Max(1, page);
            pageSize = Math.Clamp(pageSize, 1, 200);
            //&& a.Status == "Scheduled"
            return await _db.Appointments
                .Where(a => a.PatientId == patientId && a.AppointmentDateTime >= nowUtc )
                .OrderBy(a => a.AppointmentDateTime)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
        public async Task<Appointment> UpdateAppointmentAsync(int appointmentId, UpdateDto updateDto)
        {
            var appoinment = _db.Appointments.FirstOrDefault(x => x.AppointmentId == appointmentId);
            if (appoinment != null)
            {
                appoinment.AppointmentId = appointmentId;
                appoinment.Status = updateDto.Status;
                appoinment.Notes = updateDto.Notes; 
            }
            await SaveChangesAsync();
            return appoinment;
        }

        public async Task<Appointment> GetAppointmentById(int appointmentId)
        {
            return _db.Appointments.FirstOrDefault(x => x.AppointmentId == appointmentId);

        }
    }
}
