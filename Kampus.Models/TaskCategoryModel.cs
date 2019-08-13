namespace Kampus.Models
{
    public class TaskCategoryModel : Entity
    {
        public string Name { get; set; }

        public TaskCategoryModel()
        {
        }

        public TaskCategoryModel(int id, string name) : base(id)
        {
            Name = name;
        }
    }
}