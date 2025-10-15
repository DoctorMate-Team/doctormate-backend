using doctor.Core.Entities;
using doctor.Core.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace doctor.Repository.Data.Configuration
{
    public class DiagnosisConfiguration : IEntityTypeConfiguration<Diagnosis>
    {
        public void Configure(EntityTypeBuilder<Diagnosis> builder)
        {
            #region Table Configuration
            builder.ToTable("diagnoses");
            builder.HasKey(d => d.Id);
            #endregion

            #region Relationships
            // Relation with MedicalRecord
            builder.HasOne<MedicalRecord>()
                   .WithMany()
                   .HasForeignKey(d => d.MedicalRecordId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Optional relation with Appointment
            builder.HasOne<Appointment>()
                   .WithMany()
                   .HasForeignKey(d => d.AppointmentId)
                   .IsRequired(false)
                   .OnDelete(DeleteBehavior.SetNull);

            // Relation with User (Diagnosed by Doctor)
            builder.HasOne<User>()
                   .WithMany()
                   .HasForeignKey(d => d.DiagnosedBy)
                   .OnDelete(DeleteBehavior.Restrict);
            #endregion

            #region Properties
            builder.Property(d => d.DiagnosisText)
                   .HasColumnType("text")
                   .IsRequired(false);

            builder.Property(d => d.IcdCode)
                   .HasMaxLength(20)
                   .IsRequired(false);

            builder.Property(d => d.Severity)
                   .HasMaxLength(20)
                   .IsRequired(false);

            builder.Property(d => d.OpenmrsObsUuid)
                   .HasMaxLength(64)
                   .IsRequired(false);

            builder.Property(d => d.CreatedAt)
                   .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(d => d.UpdatedAt)
                   .IsRequired(false);
            #endregion

            #region Indexes
            builder.HasIndex(d => d.MedicalRecordId);
            builder.HasIndex(d => d.AppointmentId);
            builder.HasIndex(d => d.DiagnosedBy);
            builder.HasIndex(d => d.IcdCode);
            #endregion

        }
    }
}
