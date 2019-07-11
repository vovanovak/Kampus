namespace Kampus.Models
{
    public class TaskSubscriberModel : Entity
    {
        public int? Price { get; set; }

        public UserShortModel Subscriber { get; set; }
    }
}
