using Kampus.Models;
using Kampus.Persistence.Entities.UniversityRelated;
using System.Linq;

namespace Kampus.Application.Mappers.Impl
{
    internal class UniversityMapper : IUniversityMapper
    {
        public UniversityModel Map(University university)
        {
            return new UniversityModel
            {
                Id = university.UniversityId,
                Name = university.Name,
                Faculties = university.Faculties
                    .Select(f => new UniversityFacultyModel { Id = f.FacultyId, Name = f.Name })
                    .ToList()
            };
        }
    }
}
