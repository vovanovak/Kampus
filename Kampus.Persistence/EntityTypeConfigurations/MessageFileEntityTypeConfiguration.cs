using Kampus.Persistence.Entities.AttachmentsRelated;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kampus.Persistence.EntityTypeConfigurations
{
    public class MessageFileEntityTypeConfiguration : IEntityTypeConfiguration<MessageFile>
    {
        public void Configure(EntityTypeBuilder<MessageFile> builder)
        {
            builder.HasKey(mf => mf.MessageFileId);
            builder.HasOne(mf => mf.File);
            builder.HasOne(mf => mf.Message).WithMany(m => m.Attachments);
        }
    }
}