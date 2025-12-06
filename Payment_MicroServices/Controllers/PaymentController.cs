using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Payment_MicroServices.DTOs;
using Payment_MicroServices.Model;
using Payment_MicroServices.Repository;
using Razorpay.Api;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Payment_MicroServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentRepository _repo;
        private readonly IConfiguration _config;

        public PaymentController(IPaymentRepository repo, IConfiguration config)
        {
            _repo = repo;
            _config = config;
        }

        // POST: api/payments/create-order
        [HttpPost("create-order")]
        public async Task<IActionResult> CreateOrder([FromBody] CreatePaymentRequest request)
        {
            if (request.AmountInRupees <= 0) return BadRequest("Amount must be > 0");

            var keyId = _config["Razorpay:KeyId"];
            var keySecret = _config["Razorpay:KeySecret"];
            if (string.IsNullOrWhiteSpace(keyId) || string.IsNullOrWhiteSpace(keySecret))
                return StatusCode(500, "Razorpay keys not configured.");

            // convert to paise
            long amountInPaise = (long)request.AmountInRupees * 100;

            var payment = new PaymentEntity
            {
                PatientId = request.PatientId,
                DoctorId = request.DoctorId,
                AmountBigint = amountInPaise,
                Status = "created",
                PayerName = request.PayerName,
                PayerEmail = request.PayerEmail,
                PayerPhone = request.PayerPhone,
                Notes = request.Notes
            };

            payment = await _repo.CreateAsync(payment); // now has PaymentId

            // 2) create Razorpay order
            var client = new RazorpayClient(keyId, keySecret);
            var options = new Dictionary<string, object>
            {
                { "amount", amountInPaise },
                { "currency", "INR" },
                { "receipt", $"payment_{payment.PaymentId}" },
                { "payment_capture", 1 }
            };

            Order order = client.Order.Create(options);
            var orderId = order["id"].ToString();

            // 3) update payment with GatewayOrderId and set status -> pending
            payment.GatewayOrderId = orderId;
            payment.Status = "pending";
            await _repo.UpdateAsync(payment);

            // 4) return response for frontend
            var resp = new CreatePaymentResponse
            {
                PaymentId = payment.PaymentId,
                OrderId = orderId,
                RazorpayKey = keyId,
                AmountInPaise = amountInPaise,
                Currency = "INR"
            };

            return Ok(resp);
        }

        // POST: api/payments/verify
        [HttpPost("verify")]
        public async Task<IActionResult> Verify([FromBody] VerifyPaymentRequest request)
        {
            var keySecret = _config["Razorpay:KeySecret"];
            if (string.IsNullOrWhiteSpace(keySecret))
                return StatusCode(500, "Razorpay keys not configured.");

            var payment = await _repo.GetByIdAsync(request.PaymentId);
            if (payment == null) return NotFound("Payment not found");

            if (!string.Equals(payment.GatewayOrderId, request.RazorpayOrderId, StringComparison.OrdinalIgnoreCase))
                return BadRequest("Order ID mismatch");

            // compute expected signature
            var payload = request.RazorpayOrderId + "|" + request.RazorpayPaymentId;
            var expected = ComputeHmacSha256(payload, keySecret);

            if (!string.Equals(expected, request.RazorpaySignature, StringComparison.OrdinalIgnoreCase))
            {
                payment.Status = "failed";
                await _repo.UpdateAsync(payment);
                return BadRequest("Invalid signature");
            }

            // mark captured
            payment.GatewayPaymentId = request.RazorpayPaymentId;
            payment.GatewaySignature = request.RazorpaySignature;
            payment.Status = "captured";
            payment.PaidAt = DateTime.UtcNow;

            await _repo.UpdateAsync(payment);

            return Ok(new { message = "Payment verified", paymentId = payment.PaymentId, status = payment.Status });
        }

        // GET: api/payments/patient/123
        [HttpGet("patient/{patientId:int}")]
        public async Task<IActionResult> GetByPatient(int patientId)
        {
            var items = await _repo.GetByPatientAsync(patientId);
            return Ok(items);
        }

        // helper
        private static string ComputeHmacSha256(string text, string secret)
        {
            var keyBytes = Encoding.UTF8.GetBytes(secret);
            var textBytes = Encoding.UTF8.GetBytes(text);

            using var hmac = new HMACSHA256(keyBytes);
            var hash = hmac.ComputeHash(textBytes);
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }
    }
}
