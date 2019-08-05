using Kampus.Persistence.Entities.UserRelated;
using System;

namespace Kampus.Persistence.Entities.WallPostRelated
{
    public class WallPostComment
    {
        public int WallPostCommentId { get; set; }
        public string Content { get; set; }

        public int CreatorId { get; set; }
        public User Creator { get; set; }

        public int WallPostId { get; set; }
        public WallPost WallPost { get; set; }

        public DateTime CreationTime { get; set; }
    }
}
