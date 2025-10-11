using doctor.Core.Entities;
using doctor.Core.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace doctor.Repository.Data.Configuration
{
    public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
    {
        public void Configure(EntityTypeBuilder<Appointment> builder)
        {
            #region Table Configuration
            // Define the table name in the database
            builder.ToTable("appointments");
            #endregion

            #region Primary Key
            // Define the primary key
            builder.HasKey(a => a.Id);
            #endregion

            #region Relationship with Patient
            // Many-to-One relationship between Appointment and Patient
            builder.HasOne(a => a.Patient)
                   .WithMany()
                   .HasForeignKey(a => a.PatientId)
                   .OnDelete(DeleteBehavior.Cascade);
            #endregion

            #region Relationship with Doctor
            // Many-to-One relationship between Appointment and Doctor
            builder.HasOne(a => a.Doctor)
                   .WithMany()
                   .HasForeignKey(a => a.DoctorId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Restrict);
            #endregion

            #region Scheduling Properties
            // ScheduledStart is required
            builder.Property(a => a.ScheduledStart)
                   .IsRequired();

            // ScheduledEnd is optional
            builder.Property(a => a.ScheduledEnd)
                   .IsRequired(false);
            #endregion

            #region Appointment Status and Type
            // Status (pending, confirmed, completed, canceled)
            builder.Property(a => a.Status)
                   .HasMaxLength(32)
                   .HasDefaultValue("pending");

            // Reason (optional)
            builder.Property(a => a.Reason)
                   .HasColumnType("text")
                   .IsRequired(false);

            // Appointment type (in_person or online)
            builder.Property(a => a.AppointmentType)
                   .HasMaxLength(32)
                   .HasDefaultValue("in_person");
            #endregion

            #region Sync Properties
            // Sync status (pending, synced, failed)
            builder.Property(a => a.SyncStatus)
                   .HasMaxLength(16)
                   .HasDefaultValue("pending");
            #endregion

            #region Cancellation Details
            // CanceledAt (optional)
            builder.Property(a => a.CanceledAt)
                   .IsRequired(false);

            // CanceledBy (optional)
            builder.Property(a => a.CanceledBy)
                   .IsRequired(false);
            #endregion

            #region OpenMRS Integration
            // OpenMRS Appointment UUID (optional)
            builder.Property(a => a.OpenmrsAppointmentUuid)
                   .HasMaxLength(64)
                   .IsRequired(false);
            #endregion

            #region Timestamp Fields
            // CreatedAt (default: current UTC time)
            builder.Property(a => a.CreatedAt)
                   .HasDefaultValueSql("GETUTCDATE()");

            // UpdatedAt (optional)
            builder.Property(a => a.UpdatedAt)
                   .IsRequired(false);
            #endregion

            #region Indexes
            // Create index for DoctorId and ScheduledStart to improve query performance
            builder.HasIndex(a => new { a.DoctorId, a.ScheduledStart });
            #endregion

            #region Relationship with User (CanceledBy)
            // Many-to-One relationship between Appointment and User (who canceled the appointment)
            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(a => a.CanceledBy)
                .IsRequired(false);
            #endregion

        }
    }
}
