using System.Collections.Generic;

namespace Kampus.Persistence.Entities.UniversityRelated
{
    public class University : DbEntity
    {
        public string Name { get; set; }

        public virtual List<UniversityFaculty> Faculties { get; set; }
    }
}
