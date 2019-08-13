using System.Collections.Generic;
using System.Linq;
using Kampus.Application.Mappers;
using Kampus.Models;
using Kampus.Persistence.Contexts;
using Kampus.Persistence.Entities.UniversityRelated;
using Newtonsoft.Json;

namespace Kampus.Application.Services.Impl
{
    public class UniversityService : IUniversityService
    {
        private readonly KampusContext _context;
        private readonly IUniversityMapper _universityMapper;

        public UniversityService(KampusContext context, IUniversityMapper universityMapper)
        {
            _context = context;
            _universityMapper = universityMapper;
        }

        public int GetFacultyId(int universityId, string name)
        {
            return _context.Faculties.First(f => f.UniversityId == universityId && f.Name == name).FacultyId;
        }

        public List<UniversityModel> GetUniversities()
        {
            return _context.Universities.Select(_universityMapper.Map).ToList();
        }

        public string GetUniversityFaculties(string name)
        {
            var university = _context.Universities.First(u => u.Name == name);
            return JsonConvert.SerializeObject(university.Faculties.Select(f => new { f.FacultyId, f.Name }).ToArray());
        }
    }
}
