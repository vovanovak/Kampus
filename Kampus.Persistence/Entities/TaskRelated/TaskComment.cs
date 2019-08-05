using Kampus.Persistence.Entities.UserRelated;
using System;

namespace Kampus.Persistence.Entities.TaskRelated
{
    public class TaskComment
    {
        public int TaskCommentId { get; set; }

        public string Content { get; set; }

        public DateTime CreationTime { get; set; }

        public int CreatorId { get; set; }
        public User Creator { get; set; }

        public int TaskId { get; set; }
        public TaskEntry TaskEntry { get; set; }
    }
}
