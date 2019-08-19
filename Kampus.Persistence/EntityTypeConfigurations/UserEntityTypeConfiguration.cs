using Kampus.Persistence.Entities.UserRelated;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kampus.Persistence.EntityTypeConfigurations
{
    public class UserEntityTypeConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(ts => ts.UserId);
            builder.HasOne(u => u.StudentDetails);
            builder.HasOne(u => u.Role);
            builder.HasOne(u => u.City);
            builder.HasOne(u => u.UserPermissions);
        }
    }
}