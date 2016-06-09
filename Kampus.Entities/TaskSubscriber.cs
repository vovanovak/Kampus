namespace Kampus.Entities
{
    public class TaskSubscriber : DbEntity
    {
        public int? UserId { get; set; }
        public User User { get; set; }

        public int? Price { get; set; }
    }

}