using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Linq.Expressions;
using Kampus.DAL.Abstract;
using Kampus.Entities;
using Kampus.Models;
using Newtonsoft.Json;

namespace Kampus.DAL.Concrete
{
    public class UniversityRepositoryBase: RepositoryBase<UniversityModel, University>, IUniversityRepository
    {
        protected override DbSet<University> GetTable()
        {
            return ctx.Universities;
        }

        protected override Expression<Func<University, UniversityModel>> GetConverter()
        {
            return u => new UniversityModel()
            {
                Id = u.Id,
                Name = u.Name,
                Faculties = u.Faculties.Select(f => new UniversityFacultyModel() { Id = f.Id, Name = f.Name}).ToList()
            };
        }

        protected override void UpdateEntry(University dbEntity, UniversityModel entity)
        {
            dbEntity.Id = entity.Id;
            dbEntity.Name = entity.Name;
        }

        public int GetFacultyId(int universityid, string name)
        {
            return ctx.Faculties.First(f => f.UniversityId == universityid && f.Name == name).Id;
        }

        public List<UniversityModel> GetUniversities()
        {

            List<UniversityModel> models = ctx.Universities.Select(GetConverter()).ToList();

            return models;
        }

        public string GetUniversityFaculties(string name)
        {
            List<University> universities = ctx.Universities.ToList();
            University university = universities.First(u => u.Name == name);
            string res = JsonConvert.SerializeObject(university.Faculties.Select(f => new { f.Id, f.Name }).ToArray());
            return res;
        }
    }
}