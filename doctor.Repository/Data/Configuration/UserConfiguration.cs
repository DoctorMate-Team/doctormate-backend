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
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            #region Table Configuration
            // Define the table name and primary key
            builder.ToTable("users");
            builder.HasKey(u => u.Id);
            #endregion

            #region Properties
            // Configure basic user properties
            builder.Property(u => u.Email)
                   .IsRequired()
                   .HasMaxLength(255);

            builder.Property(u => u.PasswordHash)
                   .IsRequired()
                   .HasMaxLength(255);

            builder.Property(u => u.FullName)
                   .HasMaxLength(255);

            builder.Property(u => u.Phone)
                   .HasMaxLength(12)
                   .IsRequired(false);

            builder.Property(u => u.Role)
                   .IsRequired()
                   .HasMaxLength(32);

            builder.Property(u => u.IsActive)
                   .HasDefaultValue(true);

            builder.Property(u => u.CreatedAt)
                   .HasDefaultValueSql("GETUTCDATE()");
            #endregion

            #region Indexes
            // Define indexes for uniqueness and fast lookup
            builder.HasIndex(u => u.Email).IsUnique();
            builder.HasIndex(u => u.Phone).IsUnique();
            #endregion

        }
    }
}
