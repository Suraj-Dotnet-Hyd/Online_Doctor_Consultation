namespace DoctorRegistration.DTOs
{
    public class DoctorFilterRequest
    {
        public string? Specialization { get; set; }
        public int? MinExperience { get; set; }
        public decimal? MinRating { get; set; }
        public decimal? MaxConsultationFee { get; set; }
    }
}
