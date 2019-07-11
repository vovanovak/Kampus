using Kampus.Models;
using Kampus.Persistence.Entities.UserRelated;

namespace Kampus.Application.Mappers.Impl
{
    internal class CityMapper : ICityMapper
    {
        public CityModel Map(City city) => new CityModel() { Id = city.Id, Name = city.Name };
    }
}
