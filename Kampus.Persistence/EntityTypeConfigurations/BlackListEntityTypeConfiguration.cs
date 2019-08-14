using Kampus.Persistence.Entities.UserRelated;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kampus.Persistence.EntityTypeConfigurations
{
    public class BlackListEntityTypeConfiguration : IEntityTypeConfiguration<BlackList>
    {
        public void Configure(EntityTypeBuilder<BlackList> builder)
        {
            builder.HasKey(b => b.BlackListId);
            builder.HasOne(b => b.User1).WithMany().HasForeignKey(b => b.User1Id);
            builder.HasOne(b => b.User2).WithMany().HasForeignKey(b => b.User2Id);
        }
    }
}