namespace Kampus.Models
{
    public class TaskSubcategoryModel : Entity
    {
        public string Name { get; set; }
        public int? TaskCategoryId { get; set; }

        public TaskSubcategoryModel(int id, string name, int? taskCategoryId) : base(id)
        {
            Name = name;
            TaskCategoryId = taskCategoryId;
        }
    }
}