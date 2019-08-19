using Kampus.Persistence.Entities.UserRelated;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kampus.Persistence.EntityTypeConfigurations
{
    public class UserRecoveryEntityTypeConfiguration : IEntityTypeConfiguration<UserRecovery>
    {
        public void Configure(EntityTypeBuilder<UserRecovery> builder)
        {
            builder.HasKey(ur => ur.UserRecoveryId);
            builder.HasOne(b => b.User);
        }
    }
}