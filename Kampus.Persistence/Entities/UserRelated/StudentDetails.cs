using Kampus.Persistence.Entities.UniversityRelated;

namespace Kampus.Persistence.Entities.UserRelated
{
    public class StudentDetails : DbEntity
    {
        public int? UniversityId { get; set; }
        public virtual University University { get; set; }

        public int? FacultyId { get; set; }
        public virtual UniversityFaculty Faculty { get; set; }

        public int Course { get; set; }
    }
}
