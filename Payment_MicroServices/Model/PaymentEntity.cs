namespace Payment_MicroServices.Model
{
    public class PaymentEntity
    {
        public int PaymentId { get; set; }
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public string? GatewayOrderId { get; set; }
        public string? GatewayPaymentId { get; set; }
        public string? GatewaySignature { get; set; }

        public long AmountBigint { get; set; }   // paise
        public string Status { get; set; } = "created";   // created, pending, captured, failed, refunded, cancelled

        public string? PaymentMethod { get; set; }

        public string? PayerName { get; set; }
        public string? PayerEmail { get; set; }
        public string? PayerPhone { get; set; }

        public string? Notes { get; set; }

        public DateTime? PaidAt { get; set; }
    }
}
