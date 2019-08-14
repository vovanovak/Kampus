using Kampus.Persistence.Entities.AttachmentsRelated;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kampus.Persistence.EntityTypeConfigurations
{
    public class TaskFileEntityTypeConfiguration : IEntityTypeConfiguration<TaskFile>
    {
        public void Configure(EntityTypeBuilder<TaskFile> builder)
        {
            builder.HasKey(tf => tf.TaskFileId);
            builder.HasOne(tf => tf.File);
            builder.HasOne(tf => tf.Task).WithMany(t => t.Attachments);
        }
    }
}