using System;

namespace Kampus.Models
{
    public class TaskCommentModel: Entity
    {
        public string Content { get; set; }

        public DateTime CreationTime { get; set; }

        public UserShortModel Creator { get; set; }

        public int? TaskId { get; set; }
    }
}
