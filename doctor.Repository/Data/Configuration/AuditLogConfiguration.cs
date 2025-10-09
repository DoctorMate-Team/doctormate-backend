using doctor.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace doctor.Repository.Data.Configuration
{
    public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
    {
        public void Configure(EntityTypeBuilder<AuditLog> builder)
        {
            #region Table Configuration
            builder.ToTable("audit_logs");
            builder.HasKey(al => al.Id);
            #endregion

            #region Relationships
            // Relation with User
            builder.HasOne<User>()
                   .WithMany()
                   .HasForeignKey(al => al.UserId)
                   .OnDelete(DeleteBehavior.Restrict);
            #endregion

            #region Properties
            builder.Property(al => al.Action)
                   .HasMaxLength(100)
                   .IsRequired();

            builder.Property(al => al.Entity)
                   .HasMaxLength(100)
                   .IsRequired();

            builder.Property(al => al.EntityId)
                   .IsRequired(false);

            builder.Property(al => al.Status)
                   .HasMaxLength(50)
                   .IsRequired();

            builder.Property(al => al.LogTime)
                   .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(al => al.Response)
                   .HasColumnType("nvarchar(max)")
                   .IsRequired(false);
            #endregion

            #region Indexes
            builder.HasIndex(al => al.UserId);
            builder.HasIndex(al => al.Action);
            builder.HasIndex(al => al.Entity);
            builder.HasIndex(al => al.Status);
            builder.HasIndex(al => al.LogTime);
            builder.HasIndex(al => new { al.Entity, al.EntityId });
            #endregion

        }
    }
}
