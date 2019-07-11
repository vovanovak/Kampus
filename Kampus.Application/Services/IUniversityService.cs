using Kampus.Models;
using System.Collections.Generic;

namespace Kampus.Application.Services
{
    public interface IUniversityService
    {
        int GetFacultyId(int universityid, string name);
        List<UniversityModel> GetUniversities();
        string GetUniversityFaculties(string name);
    }
}
