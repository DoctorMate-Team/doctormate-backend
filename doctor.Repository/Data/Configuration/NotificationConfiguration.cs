using doctor.Core.Entities;
using doctor.Core.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace doctor.Repository.Data.Configuration
{
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            #region Table Configuration
            // Define the table name and primary key
            builder.ToTable("notifications");
            builder.HasKey(n => n.Id);
            #endregion

            #region Relationships
            // Define the relationship with User (One-to-Many)
            builder.HasOne<User>()
                   .WithMany(u => u.Notifications)
                   .HasForeignKey(n => n.UserId)
                   .OnDelete(DeleteBehavior.Cascade);
            #endregion

            #region Properties
            // Configure entity properties and their constraints
            builder.Property(n => n.Message)
                   .HasColumnType("text")
                   .IsRequired();

            builder.Property(n => n.Type)
                   .HasMaxLength(50)
                   .IsRequired(false);

            builder.Property(n => n.IsRead)
                   .HasDefaultValue(false)
                   .IsRequired();

            builder.Property(n => n.CreatedAt)
                   .HasDefaultValueSql("GETUTCDATE()");
            #endregion

            #region Indexes
            // Define indexes to improve query performance
            builder.HasIndex(n => n.UserId);
            builder.HasIndex(n => n.IsRead);
            builder.HasIndex(n => n.CreatedAt);
            builder.HasIndex(n => new { n.UserId, n.IsRead });
            #endregion

        }
    }
}
