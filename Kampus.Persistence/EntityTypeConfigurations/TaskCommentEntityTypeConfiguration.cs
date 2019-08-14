using Kampus.Persistence.Entities.TaskRelated;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kampus.Persistence.EntityTypeConfigurations
{
    public class TaskCommentEntityTypeConfiguration : IEntityTypeConfiguration<TaskComment>
    {
        public void Configure(EntityTypeBuilder<TaskComment> builder)
        {
            builder.HasKey(tc => tc.TaskCommentId);
            builder.HasOne(tc => tc.TaskEntry).WithMany(t => t.TaskComments).HasForeignKey(tc => tc.TaskId);
            builder.HasOne(tc => tc.Creator).WithMany().HasForeignKey(tc => tc.CreatorId);
        }
    }
}