using Kampus.Persistence.Entities.AttachmentsRelated;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kampus.Persistence.EntityTypeConfigurations
{
    public class WallPostFileEntityTypeConfiguration : IEntityTypeConfiguration<WallPostFile>
    {
        public void Configure(EntityTypeBuilder<WallPostFile> builder)
        {
            builder.HasKey(wpf => wpf.WallPostFileId);
            builder.HasOne(wpf => wpf.File);
            builder.HasOne(wpf => wpf.WallPost).WithMany(wp => wp.Attachments);
        }
    }
}