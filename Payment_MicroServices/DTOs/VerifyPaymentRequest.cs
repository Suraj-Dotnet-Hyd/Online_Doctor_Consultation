namespace Payment_MicroServices.DTOs
{
    public class VerifyPaymentRequest
    {
        public int PaymentId { get; set; }
        public string RazorpayPaymentId { get; set; } = null!;
        public string RazorpayOrderId { get; set; } = null!;
        public string RazorpaySignature { get; set; } = null!;
    }
}
