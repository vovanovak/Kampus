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

        public int? UserId { get; set; }
        public virtual User User { get; set; }
    }
}
