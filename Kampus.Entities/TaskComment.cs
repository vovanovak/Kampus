using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kampus.Entities
{
    public class TaskComment : DbEntity
    {
        public string Content { get; set; }

        public DateTime CreationTime { get; set; }

        public int? CreatorId { get; set; }
        public User Creator { get; set; }

        public int? TaskId { get; set; }
        public Task Task { get; set; }
    }
}
