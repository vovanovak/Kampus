using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kampus.Entities
{
    public class TaskLike : DbEntity
    {
        

        public int? TaskId { get; set; }
        public virtual Task Task { get; set; }

        public int? LikerId { get; set; }
        public virtual User Liker { get; set; }
    }
}
