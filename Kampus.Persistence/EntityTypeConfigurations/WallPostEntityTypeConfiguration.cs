using Kampus.Persistence.Entities.WallPostRelated;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kampus.Persistence.EntityTypeConfigurations
{
    public class WallPostEntityTypeConfiguration : IEntityTypeConfiguration<WallPost>
    {
        public void Configure(EntityTypeBuilder<WallPost> builder)
        {
            builder.HasKey(wp => wp.WallPostId);
            builder.HasOne(w => w.Owner); //.WithMany().HasForeignKey(w => w.OwnerId);
            builder.HasOne(w => w.Sender); //.WithMany().HasForeignKey(w => w.SenderId);
        }
    }
}