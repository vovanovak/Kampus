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
using Kampus.DAL.Abstract.Repositories;

namespace Kampus.DAL.Concrete.Repositories
{
    internal class UniversityRepositoryBase: RepositoryBase<UniversityModel, University>, IUniversityRepository
    {
        public UniversityRepositoryBase(KampusContext context) : base(context)
        {
        }

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

        public int GetFacultyId(int universityId, string name)
        {
            return ctx.Faculties.First(f => f.UniversityId == universityId && f.Name == name).Id;
        }

        public List<UniversityModel> GetUniversities()
        {
            return ctx.Universities.Select(GetConverter()).ToList();
        }

        public string GetUniversityFaculties(string name)
        {
            University university = ctx.Universities.First(u => u.Name == name);
            return JsonConvert.SerializeObject(university.Faculties.Select(f => new { f.Id, f.Name }).ToArray());
        }
    }
}