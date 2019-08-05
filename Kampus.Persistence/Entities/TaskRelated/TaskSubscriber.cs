using Kampus.Persistence.Entities.UserRelated;

namespace Kampus.Persistence.Entities.TaskRelated
{
    public class TaskSubscriber
    {
        public int SubscriberId { get; set; }
        public User Subscriber { get; set; }

        public int TaskId { get; set; }
        public TaskEntry TaskEntry { get; set; }

        public decimal Price { get; set; }
    }
}