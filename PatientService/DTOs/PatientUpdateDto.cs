namespace PatientService.DTOs
{
    public class PatientUpdateDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PassHash { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string MedicalHistory { get; set; }
        public IFormFile? Image {  get; set; }
        public string Allergies { get; set; }
        public string BloodGroup { get; set; }

    }
}
