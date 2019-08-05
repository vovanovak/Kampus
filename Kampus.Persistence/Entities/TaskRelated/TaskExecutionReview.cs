using Kampus.Persistence.Entities.UserRelated;

namespace Kampus.Persistence.Entities.TaskRelated
{
    public class TaskExecutionReview : DbEntity
    {
        public int? TaskId { get; set; }
        public Task Task { get; set; }

        public int? ExecutorId { get; set; }
        public User Executor { get; set; }

        public float? Rating { get; set; }
        public string Review { get; set; }
    }
}
