using Kampus.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kampus.Persistence.Entities.UniversityRelated;

namespace Kampus.Application.Services
{
    public interface IUniversityService
    {
        Task<IReadOnlyList<UniversityModel>> GetUniversities();
        Task<IReadOnlyList<Faculty>> GetUniversityFaculties(string name);
    }
}
