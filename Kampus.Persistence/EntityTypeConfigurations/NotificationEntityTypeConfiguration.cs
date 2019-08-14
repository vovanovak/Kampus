using Kampus.Persistence.Entities.NotificationRelated;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kampus.Persistence.EntityTypeConfigurations
{
    public class NotificationEntityTypeConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.HasKey(n => n.NotificationId);
            builder.HasOne(n => n.Sender).WithMany().HasForeignKey(n => n.SenderId);
            builder.HasOne(n => n.Receiver).WithMany().HasForeignKey(n => n.ReceiverId);
        }
    }
}