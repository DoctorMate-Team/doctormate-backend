using doctor.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace doctor.Repository.Data.Configuration
{
    public class PrescriptionConfiguration : IEntityTypeConfiguration<Prescription>
    {
        public void Configure(EntityTypeBuilder<Prescription> builder)
        {
            #region Table Configuration
            // Define the table name and primary key
            builder.ToTable("prescriptions");
            builder.HasKey(p => p.Id);
            #endregion

            #region Relationships
            // Define the relationship with Diagnosis (many-to-one)
            builder.HasOne<Diagnosis>()
                   .WithMany()
                   .HasForeignKey(p => p.DiagnosisId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Define the relationship with Patient (many-to-one)
            builder.HasOne<Patient>()
                   .WithMany()
                   .HasForeignKey(p => p.PatientId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Define the relationship with Doctor (many-to-one)
            builder.HasOne<Doctor>()
                   .WithMany()
                   .HasForeignKey(p => p.DoctorId)
                   .OnDelete(DeleteBehavior.Restrict);
            #endregion

            #region Properties
            // Configure entity properties and their constraints
            builder.Property(p => p.DrugName)
                   .HasMaxLength(200)
                   .IsRequired();

            builder.Property(p => p.Dosage)
                   .HasMaxLength(100)
                   .IsRequired(false);

            builder.Property(p => p.Instructions)
                   .HasColumnType("text")
                   .IsRequired(false);

            builder.Property(p => p.DurationDays)
                   .IsRequired(false);

            builder.Property(p => p.OpenmrsOrderUuid)
                   .HasMaxLength(64)
                   .IsRequired(false);

            builder.Property(p => p.CreatedAt)
                   .HasDefaultValueSql("GETUTCDATE()");
            #endregion

            #region Indexes
            // Define indexes for performance optimization
            builder.HasIndex(p => p.DiagnosisId);
            builder.HasIndex(p => p.PatientId);
            builder.HasIndex(p => p.DoctorId);
            builder.HasIndex(p => p.DrugName);
            #endregion

        }
    }
}
