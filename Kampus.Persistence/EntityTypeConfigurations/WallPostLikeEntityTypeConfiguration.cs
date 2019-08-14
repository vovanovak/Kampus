using Kampus.Persistence.Entities.WallPostRelated;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kampus.Persistence.EntityTypeConfigurations
{
    public class WallPostLikeEntityTypeConfiguration : IEntityTypeConfiguration<WallPostLike>
    {
        public void Configure(EntityTypeBuilder<WallPostLike> builder)
        {
            builder.HasKey(wpl => wpl.WallPostLikeId);
            builder.HasOne(wpl => wpl.WallPost).WithMany().HasForeignKey(wpl => wpl.WallPostId);
            builder.HasOne(wpl => wpl.Liker).WithMany().HasForeignKey(wpl => wpl.LikerId);
        }
    }
}