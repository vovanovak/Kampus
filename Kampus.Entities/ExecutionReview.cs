using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kampus.Entities
{
    public class ExecutionReview: DbEntity
    {
        public int? TaskId { get; set; }
        public Task Task { get; set; }

        public int? ExecutorId { get; set; }
        public User Executor { get; set; }

        public float? Rating { get; set; }
        public string Review { get; set; }
    }
}
