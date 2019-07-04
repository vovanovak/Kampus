using Kampus.Entities;
using Kampus.Persistence.Entities.UserRelated;

namespace Kampus.Persistence.Entities.TaskRelated
{
    public class TaskLike : DbEntity
    {
        public int? TaskId { get; set; }
        public virtual Task Task { get; set; }

        public int? LikerId { get; set; }
        public virtual User Liker { get; set; }
    }
}
