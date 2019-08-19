using Kampus.Persistence.Entities.TaskRelated;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kampus.Persistence.EntityTypeConfigurations
{
    public class TaskLikeEntityTypeConfiguration : IEntityTypeConfiguration<TaskLike>
    {
        public void Configure(EntityTypeBuilder<TaskLike> builder)
        {
            builder.HasKey(tl => tl.TaskLikeId);
            builder.HasOne(tl => tl.Liker);
            builder.HasOne(tl => tl.Task).WithMany(t => t.TaskLikes).HasForeignKey(tl => tl.TaskId);
        }
    }
}