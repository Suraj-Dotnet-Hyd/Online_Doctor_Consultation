namespace Payment_MicroServices.DTOs
{
    public class CreatePaymentRequest
    {
        public int PatientId { get; set; }
        public int DoctorId { get; set; }

        // Amount in rupees from UI
        public int AmountInRupees { get; set; }

        public string? PayerName { get; set; }
        public string? PayerEmail { get; set; }
        public string? PayerPhone { get; set; }

        public string? Notes { get; set; }
    }
}
