using Kampus.Persistence.Entities.UserRelated;

namespace Kampus.Persistence.Entities.GroupRelated
{
    public class GroupPostComment : DbEntity
    {
       
        public string Content { get; set; }

        public int? UserId { get; set; }
        public User User { get; set; }

        public int? GroupPostId { get; set; }
        public GroupPost GroupPost { get; set; }
    }
}
