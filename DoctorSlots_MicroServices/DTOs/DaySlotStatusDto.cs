namespace DoctorSlots_MicroServices.DTOs
{
    public class DaySlotStatusDto
    {
        public DateOnly SlotDate { get; set; }
        public Dictionary<int, int> Slots { get; set; } = new();
    }
}
