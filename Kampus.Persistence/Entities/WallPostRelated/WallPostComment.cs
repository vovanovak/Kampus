using Kampus.Persistence.Entities.UserRelated;
using System;

namespace Kampus.Persistence.Entities.WallPostRelated
{
    public class WallPostComment : DbEntity
    {
        public string Content { get; set; }

        public int CreatorId { get; set; }
        public virtual User Creator { get; set; }

        public int WallPostId { get; set; }
        public virtual WallPost WallPost { get; set; }

        public DateTime CreationTime { get; set; }
    }
}
