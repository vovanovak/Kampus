using Microsoft.Extensions.DependencyInjection;

namespace Kampus.Persistence
{
    public static class PersistenceDependencyInjectionExtensions
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services)
        {
            return services;
        }
    }
}
