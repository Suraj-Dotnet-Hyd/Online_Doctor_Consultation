using Microsoft.EntityFrameworkCore;
using Payment_MicroServices.Model;

namespace Payment_MicroServices.Repository
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly AppDbContext _db;

        public PaymentRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<PaymentEntity> CreateAsync(PaymentEntity payment)
        {
            _db.Payments.Add(payment);
            await _db.SaveChangesAsync();
            return payment;
        }

        public async Task<PaymentEntity?> GetByIdAsync(int paymentId)
        {
            return await _db.Payments
                .FirstOrDefaultAsync(p => p.PaymentId == paymentId);
        }

        public async Task<PaymentEntity?> GetByGatewayOrderIdAsync(string gatewayOrderId)
        {
            return await _db.Payments
                .FirstOrDefaultAsync(p => p.GatewayOrderId == gatewayOrderId);
        }

        public async Task<IEnumerable<PaymentEntity>> GetByPatientAsync(int patientId)
        {
            return await _db.Payments
                .Where(p => p.PatientId == patientId)
                .OrderByDescending(p => p.PaymentId)
                .ToListAsync();
        }

        public async Task UpdateAsync(PaymentEntity payment)
        {
            _db.Payments.Update(payment);
            await _db.SaveChangesAsync();
        }
    }
}
