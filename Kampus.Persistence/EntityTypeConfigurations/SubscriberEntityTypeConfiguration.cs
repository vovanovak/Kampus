using Kampus.Persistence.Entities.UserRelated;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kampus.Persistence.EntityTypeConfigurations
{
    public class SubscriberEntityTypeConfiguration : IEntityTypeConfiguration<Subscriber>
    {
        public void Configure(EntityTypeBuilder<Subscriber> builder)
        {
            builder.HasKey(s => s.SubscriberId);
            builder.HasOne(b => b.User1).WithMany().HasForeignKey(b => b.User1Id);
            builder.HasOne(b => b.User2).WithMany().HasForeignKey(b => b.User2Id);
        }
    }
}