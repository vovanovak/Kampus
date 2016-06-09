using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kampus.Entities
{
    public class WallPostComment: DbEntity
    {
        public string Content { get; set; }

        public int UserId { get; set; }
        public virtual User User { get; set; }

        public int WallPostId { get; set; }
        public virtual WallPost WallPost { get; set; }

        public DateTime CreationTime { get; set; }
    }
}
