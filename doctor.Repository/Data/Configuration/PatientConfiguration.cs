using doctor.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace doctor.Repository.Data.Configuration
{
    public class PatientConfiguration : IEntityTypeConfiguration<Patient>
    {
        public void Configure(EntityTypeBuilder<Patient> builder)
        {
            #region Table Configuration
            // Define the table name and primary key
            builder.ToTable("patients");
            builder.HasKey(p => p.Id);
            #endregion

            #region Relationships
            // Define the one-to-one relationship with User
            builder.HasOne(p => p.User)
                   .WithOne(u => u.Patient)
                   .HasForeignKey<Patient>(p => p.UserId)
                   .OnDelete(DeleteBehavior.Cascade);
            #endregion

            #region Properties
            // Configure entity properties and their constraints
            builder.Property(p => p.Gender)
                   .HasMaxLength(10)
                   .IsRequired(false);

            builder.Property(p => p.Address)
                   .HasColumnType("text")
                   .IsRequired(false);

            builder.Property(p => p.BloodType)
                   .HasMaxLength(5)
                   .IsRequired(false);

            builder.Property(p => p.EmergencyContact)
                   .HasColumnType("nvarchar(max)")
                   .IsRequired(false);

            builder.Property(p => p.OpenmrsPatientUuid)
                   .HasMaxLength(64)
                   .IsRequired(false);

            builder.Property(p => p.CreatedAt)
                   .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(p => p.UpdatedAt)
                   .IsRequired(false);
            #endregion

            #region Indexes
            // Define indexes for better query performance
            builder.HasIndex(p => p.UserId).IsUnique();
            #endregion

        }
    }
}
