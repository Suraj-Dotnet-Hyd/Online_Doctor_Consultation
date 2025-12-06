namespace PatientService.DTOs
{
    public class PatientCreateDto
    {
 
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string PassHash { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string MedicalHistory { get; set; }
        public string Allergies { get; set; }
        public string BloodGroup { get; set; }
        //public string InsuranceProvider { get; set; }
        //public string InsuranceNumber { get; set; }
    }
}
