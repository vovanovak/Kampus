using Kampus.Entities;

namespace Kampus.Persistence.Entities.TaskRelated
{
    public class TaskSubcat : DbEntity
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public int? TaskCategoryId { get; set; }
        public TaskCategory TaskCategory { get; set; }
    }
}