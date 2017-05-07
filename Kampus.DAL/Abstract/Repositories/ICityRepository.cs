using System.Collections.Generic;
using Kampus.Models;

namespace Kampus.DAL.Abstract.Repositories
{
    public interface ICityRepository: IRepository<CityModel>
    {
        List<CityModel> GetCities();
    }
}