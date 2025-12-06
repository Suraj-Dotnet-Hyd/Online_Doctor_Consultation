namespace Payment_MicroServices.DTOs
{
    public class CreatePaymentResponse
    {
        public int PaymentId { get; set; }
        public string OrderId { get; set; } = null!;
        public string RazorpayKey { get; set; } = null!;
        public long AmountInPaise { get; set; }
        public string Currency { get; set; } = "INR";
    }
}
