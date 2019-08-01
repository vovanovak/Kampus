using Kampus.Persistence.Entities.UserRelated;

namespace Kampus.Persistence.Entities.GroupRelated
{
    public class GroupPostLike : DbEntity
    {
       

        public int? GroupPostId { get; set; }
        public virtual GroupPost Post { get; set; }

        public int? UserId { get; set; }
        public virtual User User { get; set; }
    }
}
