using Kampus.Persistence.Entities;
using Kampus.Persistence.Entities.UserRelated;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kampus.Entities
{
    public class GroupPostLike : DbEntity
    {
       

        public int? GroupPostId { get; set; }
        public virtual GroupPost Post { get; set; }

        public int? UserId { get; set; }
        public virtual User User { get; set; }
    }
}
