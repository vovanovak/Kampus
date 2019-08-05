namespace Kampus.Persistence.Entities.UserRelated
{
    public class UserRecovery
    {
        public int UserId { get; set; }
        public User User { get; set; }

        public string HashString { get; set; }
    }
}