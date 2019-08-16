using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kampus.Application.Mappers;
using Kampus.Models;
using Kampus.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Kampus.Application.Services.Impl
{
    internal class CityService : ICityService
    {
        private readonly KampusContext _context;
        private readonly ICityMapper _cityMapper;

        public CityService(KampusContext context, ICityMapper cityMapper)
        {
            _context = context;
            _cityMapper = cityMapper;
        }

        public async Task<IReadOnlyList<CityModel>> GetCities()
        {
            return await _context.Cities.Select(c => _cityMapper.Map(c)).ToListAsync();
        }
    }
}
