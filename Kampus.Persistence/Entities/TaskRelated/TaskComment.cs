using Kampus.Persistence.Entities.UserRelated;
using System;

namespace Kampus.Persistence.Entities.TaskRelated
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
