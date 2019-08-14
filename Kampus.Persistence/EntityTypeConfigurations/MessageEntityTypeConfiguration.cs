using Kampus.Persistence.Entities.MessageRelated;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kampus.Persistence.EntityTypeConfigurations
{
    public class MessageEntityTypeConfiguration : IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> builder)
        {
            builder.HasKey(m => m.MessageId);
            builder.HasOne(m => m.Sender).WithMany().HasForeignKey(m => m.SenderId);
            builder.HasOne(m => m.Receiver).WithMany().HasForeignKey(m => m.ReceiverId);
        }
    }
}