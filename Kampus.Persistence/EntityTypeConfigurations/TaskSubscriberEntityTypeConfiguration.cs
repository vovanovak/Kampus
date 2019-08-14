using Kampus.Persistence.Entities.TaskRelated;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kampus.Persistence.EntityTypeConfigurations
{
    public class TaskSubscriberEntityTypeConfiguration : IEntityTypeConfiguration<TaskSubscriber>
    {
        public void Configure(EntityTypeBuilder<TaskSubscriber> builder)
        {
            builder.HasKey(ts => ts.TaskSubscriberId);
            builder.HasOne(ts => ts.Subscriber).WithMany().HasForeignKey(ts => ts.TaskSubscriberId);
            builder.HasOne(ts => ts.TaskEntry).WithMany(t => t.TaskSubscribers).HasForeignKey(ts => ts.TaskId);
        }
    }
}