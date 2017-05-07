using System.Collections.Generic;
using Kampus.Models;

namespace Kampus.DAL.Abstract.Repositories
{
    public interface IUniversityRepository: IRepository<UniversityModel>
    {
        int GetFacultyId(int universityid, string name);
        List<UniversityModel> GetUniversities();
        string GetUniversityFaculties(string name);
    }
}