using Kampus.Persistence.Entities.UniversityRelated;

namespace Kampus.Persistence.Entities.UserRelated
{
    public class StudentDetails
    {
        public int StudentDetailsId { get; set; }

        public int UniversityId { get; set; }
        public University University { get; set; }

        public int FacultyId { get; set; }
        public Faculty Faculty { get; set; }

        public int Course { get; set; }
    }
}
