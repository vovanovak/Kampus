using Kampus.Persistence.Entities.UserRelated;

namespace Kampus.Persistence.Entities.TaskRelated
{
    public class TaskSubscriber : DbEntity
    {
        public int? SubscriberId { get; set; }
        public User Subscriber { get; set; }

        public int? Price { get; set; }
    }
}