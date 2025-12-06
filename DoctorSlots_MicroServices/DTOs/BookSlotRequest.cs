namespace DoctorSlots_MicroServices.DTOs
{
    public class BookSlotRequest
    {
        public DateOnly Date { get; set; }
        public int Hour { get; set; }
    }
}
