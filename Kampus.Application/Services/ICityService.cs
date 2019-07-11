using Kampus.Models;
using System.Collections.Generic;

namespace Kampus.Application.Services
{
    public interface ICityService
    {
        List<CityModel> GetCities();
    }
}
