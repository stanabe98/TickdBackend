using Microsoft.EntityFrameworkCore;
using TickdBackend.Application.Models.Database;

namespace TickdBackend.Application.Context

{
    public class TickdDbContext : DbContext
    {
        public TickdDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure primary key for TickdUsers
            modelBuilder.Entity<TickdUsers>()
                .HasKey(u => u.AccountId);
            modelBuilder.Entity<MeterReadings>()
                .HasKey(m => m.AccountId );

            modelBuilder.Entity<UserMeterReadings>(u =>
            {
                u.HasNoKey();
                u.ToView("UserMeterReadings");
            });

            modelBuilder.Entity<MeterReadings>()
          .HasOne<TickdUsers>()
          .WithMany()
          .HasForeignKey(m => m.AccountId);
        }

        public DbSet<TickdUsers> TickdUsers { get; set; }
        public DbSet<MeterReadings> MeterReadings { get; set; }
        public DbSet<UserMeterReadings> UserMeterReadings { get; set; }
    }
}