using doctor.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace doctor.Repository.Data.Configuration
{
    public class DoctorConfiguration : IEntityTypeConfiguration<Doctor>
    {
        public void Configure(EntityTypeBuilder<Doctor> builder)
        {
            #region Table Configuration
            builder.ToTable("doctors");
            builder.HasKey(d => d.Id);
            #endregion

            #region Relationships
            // One-to-One relation with User
            builder.HasOne(d => d.User)
                   .WithOne(u => u.Doctor)
                   .HasForeignKey<Doctor>(d => d.UserId)
                   .OnDelete(DeleteBehavior.Cascade);
            #endregion

            #region Properties
            builder.Property(d => d.Specialty)
                   .HasMaxLength(100)
                   .IsRequired(false);

            builder.Property(d => d.Qualifications)
                   .HasColumnType("text")
                   .IsRequired(false);

            builder.Property(d => d.LicenseNumber)
                   .HasMaxLength(50)
                   .IsRequired(false);

            builder.Property(d => d.ConsultationFee)
                   .HasColumnType("decimal(10,2)")
                   .IsRequired(false);

            builder.Property(d => d.OpenmrsProviderUuid)
                   .HasMaxLength(64)
                   .IsRequired(false);

            builder.Property(d => d.CreatedAt)
                   .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(d => d.UpdatedAt)
                   .IsRequired(false);
            #endregion

            #region Indexes
            builder.HasIndex(d => d.UserId).IsUnique();
            builder.HasIndex(d => d.LicenseNumber).IsUnique();
            #endregion

        }
    }
}
