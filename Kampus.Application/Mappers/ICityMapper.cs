using Kampus.Models;
using Kampus.Persistence.Entities.UserRelated;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kampus.Application.Mappers
{
    public interface ICityMapper
    {
        CityModel Map(City city);
    }
}
