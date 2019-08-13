using Kampus.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Kampus.Persistence
{
    public static class PersistenceDependencyInjectionExtensions
    {
        public static IServiceCollection AddPersistenceDependencies(this IServiceCollection services)
        {
            services.AddDbContext<KampusContext>((sp, options) =>
            {
                var configuration = sp.GetService<IConfiguration>();
                var connectionString = configuration.GetConnectionString("Sql"); 

                options.UseSqlServer(connectionString);
            });
            

            return services;
        }
    }
}
