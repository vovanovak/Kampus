using Kampus.Persistence.Entities.UserRelated;

namespace Kampus.Persistence.Entities.TaskRelated
{
    public class TaskExecutionReview
    {
        public int TaskExecutionReviewId { get; set; }

        public int TaskId { get; set; }
        public TaskEntry TaskEntry { get; set; }

        public int ExecutorId { get; set; }
        public User Executor { get; set; }

        public float? Rating { get; set; }
        public string Review { get; set; }
    }
}
