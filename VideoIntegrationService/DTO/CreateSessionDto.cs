using System.ComponentModel.DataAnnotations;

namespace VideoIntegrationService.DTO
{
    public record CreateSessionDto(
        int AppointmentId,
        int DoctorId,
        int PatientId,
        DateTime ScheduledStart,
        string JitsiBaseUrl // e.g. "https://meet.jit.si"
    );
}
