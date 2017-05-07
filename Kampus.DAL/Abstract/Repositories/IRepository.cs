using System.Collections.Generic;
using Kampus.Entities;
using Kampus.Models;

namespace Kampus.DAL.Abstract.Repositories
{
    public interface IRepository<TEntity> where TEntity : Entity
    {
        List<TEntity> GetAll();
        bool Save(TEntity entity);
        bool Delete(int id);
        bool Delete(TEntity entity);
        DbEntity GetDbEntityById(int id);
        TEntity GetEntityById(int id);
    }
}
