using Kampus.Models;
using Kampus.Persistence.Entities.UniversityRelated;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kampus.Application.Mappers
{
    public interface IUniversityMapper
    {
        UniversityModel Map(University university);
    }
}
