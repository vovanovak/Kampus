using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kampus.Entities
{
    public class WallPost: DbEntity
    {
        public string Content { get; set; }

        public int? OwnerId { get; set; }
        public virtual User Owner { get; set; }

        public int? SenderId { get; set; }
        public virtual User Sender { get; set; }

        public virtual List<WallPostComment> Comments { get; set; }
        public virtual List<WallPostLike> Likes { get; set; }
        public virtual List<File> Attachments { get; set; }

        
    }
    
}
