using System.Collections.Generic;

namespace Kampus.Persistence.Entities.TaskRelated
{
    public class TaskCategory : DbEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public virtual List<TaskSubcat> Subcategories { get; set; }
    }
}
