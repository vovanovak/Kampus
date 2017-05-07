namespace Kampus.Entities
{
    public class TaskSubcat: DbEntity
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public int? TaskCategoryId { get; set; }
        public TaskCategory TaskCategory { get; set; }
    }
}