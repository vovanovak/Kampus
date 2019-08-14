using Kampus.Persistence.Entities.TaskRelated;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kampus.Persistence.EntityTypeConfigurations
{
    public class TaskSubcategoryEntityTypeConfiguration : IEntityTypeConfiguration<TaskSubcategory>
    {
        public void Configure(EntityTypeBuilder<TaskSubcategory> builder)
        {
            builder.HasKey(ts => ts.TaskSubcategoryId);
            builder.HasOne(ts => ts.TaskCategory).WithMany(tc => tc.Subcategories).HasForeignKey(ts => ts.TaskCategoryId);
        }
    }
}