using doctor.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace doctor.Repository.Data.Configuration
{
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            #region Table Configuration
            // Define the table name and primary key
            builder.ToTable("payments");
            builder.HasKey(p => p.Id);
            #endregion

            #region Relationships
            // Define the relationship with Appointment (many-to-one)
            builder.HasOne<Appointment>()
                   .WithMany()
                   .HasForeignKey(p => p.AppointmentId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Define the relationship with Patient (many-to-one)
            builder.HasOne<Patient>()
                   .WithMany()
                   .HasForeignKey(p => p.PatientId)
                   .OnDelete(DeleteBehavior.Restrict);
            #endregion

            #region Properties
            // Configure entity properties and their constraints
            builder.Property(p => p.Amount)
                   .HasColumnType("decimal(10,2)")
                   .IsRequired();

            builder.Property(p => p.Currency)
                   .HasMaxLength(5)
                   .HasDefaultValue("USD")
                   .IsRequired();

            builder.Property(p => p.Method)
                   .HasMaxLength(50)
                   .IsRequired();

            builder.Property(p => p.Status)
                   .HasMaxLength(20)
                   .HasDefaultValue("pending")
                   .IsRequired();

            builder.Property(p => p.TransactionRef)
                   .HasMaxLength(100)
                   .IsRequired(false);

            builder.Property(p => p.PaidAt)
                   .IsRequired(false);

            builder.Property(p => p.CreatedAt)
                   .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(p => p.UpdatedAt)
                   .IsRequired(false);
            #endregion

            #region Indexes
            // Define indexes for performance and data integrity
            builder.HasIndex(p => p.AppointmentId);
            builder.HasIndex(p => p.PatientId);
            builder.HasIndex(p => p.Status);
            builder.HasIndex(p => p.TransactionRef).IsUnique();
            #endregion

        }
    }
}
