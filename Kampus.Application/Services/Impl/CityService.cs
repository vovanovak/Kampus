using System.Collections.Generic;
using System.Linq;
using Kampus.Application.Mappers;
using Kampus.Models;
using Kampus.Persistence.Contexts;

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

        public List<CityModel> GetCities()
        {
            return _context.Cities.Select(_cityMapper.Map).ToList();
        }
    }
}
