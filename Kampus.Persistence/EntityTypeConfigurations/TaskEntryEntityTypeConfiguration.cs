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
            builder.HasOne(t => t.TaskCategory);
            builder.HasOne(t => t.TaskSubcategory);
            builder.HasOne(t => t.Creator);
            builder.HasOne(t => t.Executive);
        }
    }
}