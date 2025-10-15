using doctor.Core.Entities;
using doctor.Core.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace doctor.Repository.Data.Configuration
{
    public class ChatMessageConfiguration : IEntityTypeConfiguration<ChatMessage>
    {
        public void Configure(EntityTypeBuilder<ChatMessage> builder)
        {
            #region Table Configuration
            builder.ToTable("chat_messages");
            builder.HasKey(cm => cm.Id);
            #endregion

            #region Relationships
            // Relation with User (Sender)
            builder.HasOne<User>()
                   .WithMany()
                   .HasForeignKey(cm => cm.SenderId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Relation with User (Receiver)
            builder.HasOne<User>()
                   .WithMany()
                   .HasForeignKey(cm => cm.ReceiverId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Optional relation with Appointment
            builder.HasOne<Appointment>()
                   .WithMany()
                   .HasForeignKey(cm => cm.AppointmentId)
                   .IsRequired(false)
                   .OnDelete(DeleteBehavior.SetNull);
            #endregion

            #region Properties
            builder.Property(cm => cm.Message)
                   .HasColumnType("text")
                   .IsRequired();

            builder.Property(cm => cm.SentAt)
                   .HasDefaultValueSql("GETUTCDATE()");
            #endregion

            #region Indexes
            builder.HasIndex(cm => cm.SenderId);
            builder.HasIndex(cm => cm.ReceiverId);
            builder.HasIndex(cm => cm.AppointmentId);
            builder.HasIndex(cm => cm.SentAt);
            builder.HasIndex(cm => new { cm.SenderId, cm.ReceiverId, cm.SentAt });
            #endregion

        }
    }
}
