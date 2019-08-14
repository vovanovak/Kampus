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
            builder.HasOne(u => u.StudentDetails).WithMany().HasForeignKey(u => u.StudentDetailsId);
            builder.HasOne(u => u.Role).WithMany().HasForeignKey(u => u.RoleId);
            builder.HasOne(u => u.City).WithMany().HasForeignKey(u => u.CityId);
            builder.HasOne(u => u.UserPermissions).WithMany().HasForeignKey(u => u.UserPermissionsId);
        }
    }
}