using Kampus.Persistence.Entities.UserRelated;

namespace Kampus.Entities
{
    public class TaskSubscriber : DbEntity
    {
        public int? SubscriberId { get; set; }
        public User Subscriber { get; set; }

        public int? Price { get; set; }
    }
}