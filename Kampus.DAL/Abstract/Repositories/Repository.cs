using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using Kampus.Entities;
using Kampus.Models;
using Kampus.DAL.Abstract.Repositories;

namespace Kampus.DAL.Abstract.Repositories
{
    public abstract class RepositoryBase<TEntity, TDbEntity> : IRepository<TEntity> 
        where TEntity : Entity where TDbEntity : DbEntity, new()
    {
        protected KampusContext ctx;

        public RepositoryBase(KampusContext context)
        {
            ctx = context;
        }

        public List<TEntity> GetAll()
        {
            return GetTable().Select(GetConverter()).ToList();
        }

        public DbEntity GetDbEntityById(int id)
        {
            return GetTable().FirstOrDefault(t => t.Id == id);
        }

        public TEntity GetEntityById(int id)
        {
            var res = GetTable().Where(t => t.Id == id);

            return res.Select(GetConverter()).FirstOrDefault();
        }

        public bool Save(TEntity entity)
        {
            TDbEntity e;
            if (entity.Id == -1)
            {
                e = new TDbEntity();
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
            var dbEntity = GetTable().FirstOrDefault(x => x.Id == id);
            if (dbEntity == null)
                return false;
            
            GetTable().Remove(dbEntity);
            ctx.SaveChanges();

            return true;
        }

        public bool Delete(TEntity entity)
        {
            return Delete(entity.Id);
        }

        protected abstract DbSet<TDbEntity> GetTable();
        protected abstract Expression<Func<TDbEntity, TEntity>> GetConverter();
        protected abstract void UpdateEntry(TDbEntity dbEntity, TEntity entity);
    }
}
