using Kampus.Entities;

namespace Kampus.Persistence.Entities.UniversityRelated
{
    public class UniversityFaculty : DbEntity
    {
        public string Name { get; set; }

        public int? UniversityId { get; set; }
        public virtual University University { get; set; }
    }
}
