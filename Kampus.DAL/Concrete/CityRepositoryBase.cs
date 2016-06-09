using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using Kampus.DAL.Abstract;
using Kampus.Entities;
using Kampus.Models;

namespace Kampus.DAL.Concrete
{
    public class CityRepositoryBase: RepositoryBase<CityModel, City>, ICityRepository
    {
        protected override DbSet<City> GetTable()
        {
            return ctx.Cities;
        }

        protected override Expression<Func<City, CityModel>> GetConverter()
        {
            return c => new CityModel() {Id = c.Id, Name = c.Name};
        }

        protected override void UpdateEntry(City dbEntity, CityModel entity)
        {
            dbEntity.Id = entity.Id;
            dbEntity.Name = entity.Name;
        }

        public List<CityModel> GetCities()
        {
            return ctx.Cities.Select(GetConverter()).ToList();
        }
    }
}