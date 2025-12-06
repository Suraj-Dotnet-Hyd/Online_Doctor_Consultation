using Microsoft.EntityFrameworkCore;

namespace Payment_MicroServices.Model
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
       : base(options) { }

        public DbSet<PaymentEntity> Payments { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PaymentEntity>(b =>
            {
                b.ToTable("Payments");
                b.HasKey(p => p.PaymentId);

                b.Property(p => p.PatientId).IsRequired();
                b.Property(p => p.DoctorId).IsRequired();
                b.Property(p => p.AmountBigint).IsRequired();
                b.Property(p => p.Status).HasMaxLength(50).IsRequired();
                b.Property(p => p.PaymentMethod).HasMaxLength(50);
                b.Property(p => p.GatewayOrderId).HasMaxLength(100);
                b.Property(p => p.GatewayPaymentId).HasMaxLength(100);
                b.Property(p => p.GatewaySignature).HasMaxLength(200);
            });
        }
    }
}
