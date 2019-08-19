using Kampus.Persistence.Entities.TaskRelated;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kampus.Persistence.EntityTypeConfigurations
{
    public class TaskExecutionReviewEntityTypeConfiguration : IEntityTypeConfiguration<TaskExecutionReview>
    {
        public void Configure(EntityTypeBuilder<TaskExecutionReview> builder)
        {
            builder.HasKey(ter => ter.TaskExecutionReviewId);
            builder.HasOne(ter => ter.Executor);
            builder.HasOne(ter => ter.Task);
        }
    }
}