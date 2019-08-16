using Kampus.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kampus.Application.Services
{
    public interface ICityService
    {
        Task<IReadOnlyList<CityModel>> GetCities();
    }
}
