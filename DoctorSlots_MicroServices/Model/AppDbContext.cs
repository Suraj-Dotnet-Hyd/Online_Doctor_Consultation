using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DoctorSlots_MicroServices.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Existing Doctor table (assumed to exist in your model)
        public DbSet<Doctor> Doctors { get; set; } = null!;

        // Seven per-day slot tables
        public DbSet<MondaySlots> MondaySlots { get; set; } = null!;
        public DbSet<TuesdaySlots> TuesdaySlots { get; set; } = null!;
        public DbSet<WednesdaySlots> WednesdaySlots { get; set; } = null!;
        public DbSet<ThursdaySlots> ThursdaySlots { get; set; } = null!;
        public DbSet<FridaySlots> FridaySlots { get; set; } = null!;
        public DbSet<SaturdaySlots> SaturdaySlots { get; set; } = null!;
        public DbSet<SundaySlots> SundaySlots { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Map each entity to its table name
            modelBuilder.Entity<MondaySlots>().ToTable("MondaySlots");
            modelBuilder.Entity<TuesdaySlots>().ToTable("TuesdaySlots");
            modelBuilder.Entity<WednesdaySlots>().ToTable("WednesdaySlots");
            modelBuilder.Entity<ThursdaySlots>().ToTable("ThursdaySlots");
            modelBuilder.Entity<FridaySlots>().ToTable("FridaySlots");
            modelBuilder.Entity<SaturdaySlots>().ToTable("SaturdaySlots");
            modelBuilder.Entity<SundaySlots>().ToTable("SundaySlots");

            // Configure SlotDate -> SQL date using ValueConverter for DateOnly (if using DateOnly)
            var dateOnlyConverter = new ValueConverter<DateOnly, DateTime>(
                d => d.ToDateTime(TimeOnly.MinValue),
                dt => DateOnly.FromDateTime(dt));

            // Configure each day type
            Action<Type> configureDayType = t =>
            {
                var et = modelBuilder.Entity(t);
                et.Property(nameof(DoctorDaySlotsBase.SlotDate))
                  .HasConversion(dateOnlyConverter)
                  .HasColumnType("date");

                // Foreign key to Doctors.DoctorId (assumes Doctor has key DoctorId)
                et.HasOne(typeof(Doctor))
                  .WithMany() // change if Doctor has navigation property
                  .HasForeignKey(nameof(DoctorDaySlotsBase.DoctorId))
                  .OnDelete(DeleteBehavior.Cascade);
            };

            configureDayType(typeof(MondaySlots));
            configureDayType(typeof(TuesdaySlots));
            configureDayType(typeof(WednesdaySlots));
            configureDayType(typeof(ThursdaySlots));
            configureDayType(typeof(FridaySlots));
            configureDayType(typeof(SaturdaySlots));
            configureDayType(typeof(SundaySlots));

            base.OnModelCreating(modelBuilder);
        }
    }
}
