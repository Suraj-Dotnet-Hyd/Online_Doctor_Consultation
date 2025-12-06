namespace DoctorSlots_MicroServices.DTOs
{
    public class DoctorSlotWeekDto
    {
        public int DoctorId { get; set; }
        public List<DaySlotStatusDto> Days { get; set; } = new();
    }
}
