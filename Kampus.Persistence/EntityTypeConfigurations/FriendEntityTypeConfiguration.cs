using Kampus.Persistence.Entities.UserRelated;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kampus.Persistence.EntityTypeConfigurations
{
    public class FriendEntityTypeConfiguration : IEntityTypeConfiguration<Friend>
    {
        public void Configure(EntityTypeBuilder<Friend> builder)
        {
            builder.HasKey(f => f.FriendId);
            builder.HasOne(b => b.User1).WithMany().HasForeignKey(b => b.User1Id);
            builder.HasOne(b => b.User2).WithMany().HasForeignKey(b => b.User2Id);
        }
    }
}