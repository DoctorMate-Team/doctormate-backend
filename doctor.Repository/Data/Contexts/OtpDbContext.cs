using doctor.Core.Entities.Otp;
using Microsoft.EntityFrameworkCore;

namespace doctor.Repository.Data.Contexts
{
    /// <summary>
    /// Database context for OTP operations
    /// </summary>
    public class OtpDbContext : DbContext
    {
        public OtpDbContext(DbContextOptions<OtpDbContext> options) : base(options)
        {
        }

        public DbSet<OtpEntry> OtpEntries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure OtpEntry
            modelBuilder.Entity<OtpEntry>(entity =>
            {
                entity.ToTable("OtpEntries");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .HasDefaultValueSql("NEWID()");

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.OtpCode)
                    .IsRequired()
                    .HasMaxLength(10);

                entity.Property(e => e.ExpiresAt)
                    .IsRequired();

                entity.Property(e => e.IsUsed)
                    .IsRequired()
                    .HasDefaultValue(false);

                entity.Property(e => e.CreatedAt)
                    .IsRequired()
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(e => e.RecipientEmail)
                    .HasMaxLength(256);

                // Create index for faster lookups
                entity.HasIndex(e => new { e.UserId, e.OtpCode });
                entity.HasIndex(e => e.ExpiresAt);
            });
        }
    }
}
