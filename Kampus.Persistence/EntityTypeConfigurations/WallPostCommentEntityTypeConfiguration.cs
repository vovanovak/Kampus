using Kampus.Persistence.Entities.WallPostRelated;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kampus.Persistence.EntityTypeConfigurations
{
    public class WallPostCommentEntityTypeConfiguration : IEntityTypeConfiguration<WallPostComment>
    {
        public void Configure(EntityTypeBuilder<WallPostComment> builder)
        {
            builder.HasKey(wpc => wpc.WallPostCommentId);
            builder.HasOne(wpc => wpc.WallPost);
            builder.HasOne(wpc => wpc.Creator);
        }
    }
}