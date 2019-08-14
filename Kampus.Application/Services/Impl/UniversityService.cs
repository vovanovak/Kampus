using System.Collections.Generic;
using System.Linq;
using Kampus.Application.Mappers;
using Kampus.Models;
using Kampus.Persistence.Contexts;
using Kampus.Persistence.Entities.UniversityRelated;
using Microsoft.EntityFrameworkCore;
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

        private static IQueryable<University> GetUniversities(KampusContext context)
        {
            return context.Universities.Include(u => u.Faculties);
        }

        public int GetFacultyId(int universityId, string name)
        {
            return _context.Faculties.First(f => f.UniversityId == universityId && f.Name == name).FacultyId;
        }

        public List<UniversityModel> GetUniversities()
        {
            return GetUniversities(_context).Select(u => _universityMapper.Map(u)).ToList();
        }

        public string GetUniversityFaculties(string name)
        {
            var university = GetUniversities(_context).First(u => u.Name == name);
            return JsonConvert.SerializeObject(university.Faculties.Select(f => new { f.FacultyId, f.Name }).ToArray());
        }
    }
}
