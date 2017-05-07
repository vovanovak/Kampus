using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kampus.Entities
{
    public class WallPostLike: DbEntity
    {
        public int LikerId { get; set; }
        public virtual User Liker { get; set; }

        public int WallPostId { get; set; }
        public virtual WallPost WallPost { get; set; }
    }
}
