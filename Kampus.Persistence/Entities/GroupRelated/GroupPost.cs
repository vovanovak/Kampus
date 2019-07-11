using Kampus.Persistence.Entities;
using Kampus.Persistence.Entities.GroupRelated;
using Kampus.Persistence.Entities.UserRelated;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kampus.Entities
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
