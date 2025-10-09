using doctor.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace doctor.Repository.Data.Configuration
{
    public class MedicalRecordConfiguration : IEntityTypeConfiguration<MedicalRecord>
    {
        public void Configure(EntityTypeBuilder<MedicalRecord> builder)
        {
            #region Table Configuration
            // Define the table name and primary key
            builder.ToTable("medical_records");
            builder.HasKey(mr => mr.Id);
            #endregion

            #region Relationships
            // Define the relationship with Patient (Many-to-One)
            builder.HasOne(mr => mr.Patient)
                   .WithMany()
                   .HasForeignKey(mr => mr.PatientId)
                   .OnDelete(DeleteBehavior.Cascade);
            #endregion

            #region Properties
            // Configure entity properties and their constraints
            builder.Property(mr => mr.Title)
                   .HasMaxLength(200)
                   .IsRequired(false);

            builder.Property(mr => mr.Description)
                   .HasColumnType("text")
                   .IsRequired(false);

            builder.Property(mr => mr.RecordType)
                   .HasMaxLength(50)
                   .IsRequired(false);

            builder.Property(mr => mr.Status)
                   .HasMaxLength(32)
                   .IsRequired(false);

            builder.Property(mr => mr.RecordedBy)
                   .IsRequired(false);

            builder.Property(mr => mr.OpenmrsEncounterUuid)
                   .HasMaxLength(64)
                   .IsRequired(false);

            builder.Property(mr => mr.RecordedAt)
                   .IsRequired();

            builder.Property(mr => mr.CreatedAt)
                   .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(mr => mr.UpdatedAt)
                   .IsRequired(false);
            #endregion

            #region Indexes
            // Define indexes to optimize query performance
            builder.HasIndex(mr => mr.PatientId);
            builder.HasIndex(mr => mr.RecordType);
            builder.HasIndex(mr => mr.RecordedAt);
            #endregion

        }
    }
}
