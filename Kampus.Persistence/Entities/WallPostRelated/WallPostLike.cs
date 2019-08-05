using Kampus.Persistence.Entities.UserRelated;

namespace Kampus.Persistence.Entities.WallPostRelated
{
    public class WallPostLike
    {
        public int LikerId { get; set; }
        public User Liker { get; set; }

        public int WallPostId { get; set; }
        public WallPost WallPost { get; set; }
    }
}
