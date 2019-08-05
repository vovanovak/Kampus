namespace Kampus.Persistence.Entities.TaskRelated
{
    public class TaskSubcategory
    {
        public int TaskSubcategoryId { get; set; }
        public string Name { get; set; }

        public string Description { get; set; }

        public int TaskCategoryId { get; set; }
        public TaskCategory TaskCategory { get; set; }
    }
}