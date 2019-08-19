using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Kampus.Application.Extensions
{
    public static class DbSetExtensions
    {
        public static async Task RemoveRangeAsync<T>(this DbSet<T> dbSet, Expression<Func<T, bool>> condition) where T : class
        {
            var entities = await dbSet.Where(condition).ToListAsync();
            dbSet.RemoveRange(entities);
        }

        public static async Task RemoveAsync<T>(this DbSet<T> dbSet, Expression<Func<T, bool>> condition) where T : class
        {
            var entity = await dbSet.SingleAsync(condition);
            dbSet.Remove(entity);
        }
    }
}