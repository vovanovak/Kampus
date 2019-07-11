namespace Kampus.Models
{
    public class Entity
    {
        public int Id { get; set; }

        protected Entity()
        {
            Id = -1;
        }

        protected Entity(int id)
        {
            Id = id;
        }

        public bool IsNew()
        {
            return Id == -1;
        }
    }
}
