using doctor.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace doctor.Repository.Data.Configuration
{
    public class IntegrationLogConfiguration : IEntityTypeConfiguration<IntegrationLog>
    {
        public void Configure(EntityTypeBuilder<IntegrationLog> builder)
        {
            #region Table Configuration
            // Define the table name and primary key
            builder.ToTable("integration_logs");
            builder.HasKey(il => il.Id);
            #endregion

            #region Properties
            // Configure entity properties and constraints
            builder.Property(il => il.Endpoint)
                   .HasMaxLength(500)
                   .IsRequired();

            builder.Property(il => il.Method)
                   .HasMaxLength(10)
                   .IsRequired();

            builder.Property(il => il.EntityType)
                   .HasMaxLength(100)
                   .IsRequired(false);

            builder.Property(il => il.EntityId)
                   .IsRequired(false);

            builder.Property(il => il.RequestPayload)
                   .HasColumnType("nvarchar(max)")
                   .IsRequired(false);

            builder.Property(il => il.ResponsePayload)
                   .HasColumnType("nvarchar(max)")
                   .IsRequired(false);

            builder.Property(il => il.StatusCode)
                   .IsRequired();

            builder.Property(il => il.Success)
                   .IsRequired();

            builder.Property(il => il.CreatedAt)
                   .HasDefaultValueSql("GETUTCDATE()");
            #endregion

            #region Indexes
            // Define indexes to improve query performance
            builder.HasIndex(il => il.Endpoint);
            builder.HasIndex(il => il.Method);
            builder.HasIndex(il => il.EntityType);
            builder.HasIndex(il => il.StatusCode);
            builder.HasIndex(il => il.Success);
            builder.HasIndex(il => il.CreatedAt);
            builder.HasIndex(il => new { il.EntityType, il.EntityId });
            #endregion

        }
    }
}
