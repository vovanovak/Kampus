namespace Kampus.Models
{
    public class TaskSubscriberModel : Entity
    {
        public decimal? Price { get; set; }

        public UserShortModel Subscriber { get; set; }
    }
}
