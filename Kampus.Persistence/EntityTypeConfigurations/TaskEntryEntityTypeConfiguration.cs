using Kampus.Persistence.Entities.TaskRelated;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kampus.Persistence.EntityTypeConfigurations
{
    public class TaskEntryEntityTypeConfiguration : IEntityTypeConfiguration<TaskEntry>
    {
        public void Configure(EntityTypeBuilder<TaskEntry> builder)
        {
            builder.ToTable("Tasks");

            builder.HasKey(t => t.TaskId);
            builder.HasOne(t => t.TaskCategory).WithMany().HasForeignKey(t => t.TaskCategoryId);
            builder.HasOne(t => t.TaskSubcategory).WithMany().HasForeignKey(t => t.TaskSubcategoryId);
            builder.HasOne(t => t.Creator).WithMany().HasForeignKey(t => t.CreatorId);
            builder.HasOne(t => t.Executive).WithMany().HasForeignKey(t => t.ExecutiveId);
        }
    }
}