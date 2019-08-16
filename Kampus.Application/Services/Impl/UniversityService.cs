using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public async Task<IReadOnlyList<UniversityModel>> GetUniversities()
        {
            return await GetUniversities(_context)
                .Select(u => _universityMapper.Map(u))
                .ToListAsync();
        }

        public async Task<IReadOnlyList<Faculty>> GetUniversityFaculties(string name)
        {
            var university = await GetUniversities(_context).SingleOrDefaultAsync(u => u.Name == name);
            return university.Faculties;
        }
    }
}
