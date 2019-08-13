using System;
using System.Collections.Generic;
using System.Text;
using Kampus.Persistence.Entities.TaskRelated;

namespace Kampus.Persistence.Entities.AttachmentsRelated
{
    public class TaskFile
    {
        public int TaskId { get; set; }
        public TaskEntry Task { get; set; }

        public int FileId { get; set; }
        public File File { get; set; }
    }
}
