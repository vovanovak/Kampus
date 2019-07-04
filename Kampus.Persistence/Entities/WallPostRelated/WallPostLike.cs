using Kampus.Entities;
using Kampus.Persistence.Entities.UserRelated;

namespace Kampus.Persistence.Entities.WallPostRelated
{
    public class WallPostLike : DbEntity
    {
        public int LikerId { get; set; }
        public virtual User Liker { get; set; }

        public int WallPostId { get; set; }
        public virtual WallPost WallPost { get; set; }
    }
}
