using System.Collections.Generic;
using Kampus.Entities;
using Kampus.Models;

namespace Kampus.DAL.Abstract
{
    public interface IRepository<T> where T : Entity
    {
        List<T> GetAll();
        bool Save(T entity);
        bool Delete(int id);
        bool Delete(T entity);
        DbEntity GetDbEntityById(int id);
        Entity GetEntityById(int id);
    }
}
