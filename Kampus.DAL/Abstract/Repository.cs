using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using Kampus.Entities;
using Kampus.Models;

namespace Kampus.DAL.Abstract
{
    public abstract class RepositoryBase<T, DbT> : IRepository<T> where T : Entity where DbT : DbEntity, new()
    {
        protected KampusContext ctx = DbContextSingleton.Context;

        public RepositoryBase()
        {
            
        }

        public List<T> GetAll()
        {
            return GetTable().Select(GetConverter()).ToList();
        }

        public DbEntity GetDbEntityById(int id)
        {
            return GetTable().First(t => t.Id == id);
        }

        public Entity GetEntityById(int id)
        {
            var res = GetTable().Where(t => t.Id == id);

            return res.Select(GetConverter()).First();
        }

        public bool Save(T entity)
        {
            DbT e;
            if (entity.Id == -1)
            {
                e = new DbT();
            }
            else
            {
                e = GetTable().First(x => x.Id == entity.Id);
                if (e == null)
                    return false;
            }

            UpdateEntry(e, entity);

            if (entity.IsNew())
            {
                GetTable().Add(e);
            }

            ctx.SaveChanges();

            return true;
        }

        public bool Delete(int id)
        {
            var dbEntity = GetTable().SingleOrDefault(x => x.Id == id);
            if (dbEntity == null)
                return false;

            GetTable().Remove(dbEntity);
            ctx.SaveChanges();
            return true;
        }

        public bool Delete(T entity)
        {
            return Delete(entity.Id);
        }

        protected abstract DbSet<DbT> GetTable();
        protected abstract Expression<Func<DbT, T>> GetConverter();
        protected abstract void UpdateEntry(DbT dbEntity, T entity);
    }
}
