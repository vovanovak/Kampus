using System;
using System.Collections.Generic;
using System.Linq;

namespace Kampus.Api.Extensions
{
    public static class LinqExtensions
    {
        public static IEnumerable<T> DistinctBy<T, TKey>(this IEnumerable<T> enumerable, Func<T, TKey> distinctByFieldSelector)
        {
            return enumerable.GroupBy(i => distinctByFieldSelector(i)).Select(g => g.ToList().First());
        }
    }
}
