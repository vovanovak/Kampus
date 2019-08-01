using System;
using System.Collections.Generic;
using Kampus.Persistence.Entities.UserRelated;

namespace Kampus.Persistence.Entities.GroupRelated
{
    public class GroupPost : DbEntity
    {
        
        public string Content { get; set; }

        public int GroupId { get; set; }
        public virtual Group Group { get; set; }

        public int UserId { get; set; }
        public virtual User User { get; set; }

        public DateTime CreationTime { get; set; }

        public virtual List<GroupPostLike> Likes { get; set; }
        public virtual List<GroupPostComment> Comments { get; set; }
    }
}
