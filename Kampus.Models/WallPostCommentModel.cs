using System;

namespace Kampus.Models
{
    public class WallPostCommentModel: Entity
    {
        public DateTime CreationTime { get; set; }

        public string Content { get; set; }

        public UserShortModel Creator { get; set; }

        public int WallPostId { get; set; }
    }
}
