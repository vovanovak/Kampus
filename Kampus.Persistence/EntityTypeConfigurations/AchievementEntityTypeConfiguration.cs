using Kampus.Persistence.Entities.TaskRelated;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kampus.Persistence.EntityTypeConfigurations
{
    public class AchievementEntityTypeConfiguration : IEntityTypeConfiguration<Achievement>
    {
        public void Configure(EntityTypeBuilder<Achievement> builder)
        {
            builder.HasKey(a => a.AchievementId);
            builder.HasOne(a => a.User).WithMany(u => u.Achievements).HasForeignKey(a => a.AchievementId);
            builder.HasOne(a => a.TaskCategory).WithMany().HasForeignKey(a => a.TaskCategoryId);
        }
    }
}