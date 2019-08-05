using System.Collections.Generic;

namespace Kampus.Persistence.Entities.TaskRelated
{
    public class TaskCategory
    {
        public int TaskCategoryId { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }

        public virtual List<TaskSubcategory> Subcategories { get; set; }
    }
}
