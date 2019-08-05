using Kampus.Persistence.Entities.UserRelated;

namespace Kampus.Persistence.Entities.TaskRelated
{
    public class TaskLike
    {
        public int TaskId { get; set; }
        public TaskEntry TaskEntry { get; set; }

        public int LikerId { get; set; }
        public User Liker { get; set; }
    }
}
