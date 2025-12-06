using Microsoft.EntityFrameworkCore;
using VideoIntegrationService.Models;

namespace VideoIntegrationService.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<VideoSession> VideoSessions { get; set; } = null!;
        public DbSet<ChatMessage> ChatMessages { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<VideoSession>(eb =>
            {
                eb.HasKey(v => v.Id);
                eb.Property(v => v.RoomName).HasMaxLength(200).IsRequired();
                eb.Property(v => v.Status).HasMaxLength(50).IsRequired();
            });

            modelBuilder.Entity<ChatMessage>(eb =>
            {
                eb.HasKey(c => c.Id);
                eb.Property(c => c.Message).HasMaxLength(2000).IsRequired();
            });

            base.OnModelCreating(modelBuilder);
        }

    }
}
