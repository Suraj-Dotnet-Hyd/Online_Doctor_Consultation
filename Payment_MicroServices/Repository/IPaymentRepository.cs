using Payment_MicroServices.Model;

namespace Payment_MicroServices.Repository
{
    public interface IPaymentRepository
    {
        Task<PaymentEntity> CreateAsync(PaymentEntity payment);
        Task<PaymentEntity?> GetByIdAsync(int paymentId);
        Task<PaymentEntity?> GetByGatewayOrderIdAsync(string gatewayOrderId);
        Task<IEnumerable<PaymentEntity>> GetByPatientAsync(int patientId);
        Task UpdateAsync(PaymentEntity payment);
    }
}
