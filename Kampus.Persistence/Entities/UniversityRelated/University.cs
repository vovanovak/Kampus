using System.Collections.Generic;

namespace Kampus.Persistence.Entities.UniversityRelated
{
    public class University
    {
        public int UniversityId { get; set; }
        public string Name { get; set; }

        public List<Faculty> Faculties { get; set; }
    }
}
