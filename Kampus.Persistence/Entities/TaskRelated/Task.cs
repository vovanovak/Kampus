using Kampus.Persistence.Entities.AttachmentsRelated;
using Kampus.Persistence.Entities.UserRelated;
using System.Collections.Generic;

namespace Kampus.Persistence.Entities.TaskRelated
{
    public class Task : DbEntity
    {
        public string Header { get; set; }
        public string Content { get; set; }
        public int? Price { get; set; }

        public int? CreatorId { get; set; }
        public virtual User Creator { get; set; }

        public int? ExecutiveId { get; set; }
        public virtual User Executive { get; set; }

        public bool? Solved { get; set; }
        public bool? Hide { get; set; }

        public int? TaskCategoryId { get; set; }
        public virtual TaskCategory TaskCat { get; set; }

        public int? TaskSubcatId { get; set; }
        public virtual TaskSubcat TaskSubcat { get; set; }

        public virtual List<TaskSubscriber> TaskSubscribers { get; set; }

        public virtual List<TaskLike> TaskLikes { get; set; }
        public virtual List<TaskComment> TaskComments { get; set; }

        public virtual List<File> Attachments { get; set; }
    }
}
