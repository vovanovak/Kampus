using Kampus.Persistence.Entities;
using Kampus.Persistence.Entities.UserRelated;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kampus.Entities
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
