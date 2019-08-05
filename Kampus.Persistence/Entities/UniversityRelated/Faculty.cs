namespace Kampus.Persistence.Entities.UniversityRelated
{
    public class Faculty
    {
        public int FacultyId { get; set; }
        public string Name { get; set; }

        public int UniversityId { get; set; }
        public University University { get; set; }
    }
}
