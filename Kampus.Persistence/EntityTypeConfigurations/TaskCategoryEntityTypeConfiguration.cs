using Kampus.Persistence.Entities.TaskRelated;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kampus.Persistence.EntityTypeConfigurations
{
    public class TaskCategoryEntityTypeConfiguration : IEntityTypeConfiguration<TaskCategory>
    {
        public void Configure(EntityTypeBuilder<TaskCategory> builder)
        {
            builder.HasKey(tc => tc.TaskCategoryId);
            builder.HasMany(tc => tc.Subcategories).WithOne(ts => ts.TaskCategory);
        }
    }
}