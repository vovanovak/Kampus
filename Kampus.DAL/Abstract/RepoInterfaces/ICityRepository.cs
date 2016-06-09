using System.Collections.Generic;
using Kampus.Models;

namespace Kampus.DAL.Abstract
{
    public interface ICityRepository: IRepository<CityModel>
    {
        List<CityModel> GetCities();
    }
}