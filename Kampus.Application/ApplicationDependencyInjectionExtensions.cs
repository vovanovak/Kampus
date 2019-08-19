using Kampus.Application.Mappers;
using Kampus.Application.Mappers.Impl;
using Kampus.Application.Services;
using Kampus.Application.Services.Impl;
using Kampus.Application.Services.Users;
using Kampus.Application.Services.Users.Impl;
using Kampus.Host.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Kampus.Application
{
    public static class ApplicationDependencyInjectionExtensions
    {
        public static IServiceCollection AddApplicationDependencies(this IServiceCollection services)
        {
            return services.AddServices().AddMappers();
        }

        private static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<IUserConnectionsService, UserConnectionsService>();
            services.AddScoped<IUserProfileRecoveryService, UserProfileRecoveryService>();
            services.AddScoped<IUserSearchService, UserSearchService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ICityService, CityService>();
            services.AddScoped<IMessageService, MessageService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<ITaskService, TaskService>();
            services.AddScoped<IUniversityService, UniversityService>();
            services.AddScoped<IWallPostService, WallPostService>();
            services.AddScoped<IFileService, FileService>();

            return services;
        }

        private static IServiceCollection AddMappers(this IServiceCollection services)
        {
            services.AddScoped<IUserMapper, UserMapper>();
            services.AddScoped<ICityMapper, CityMapper>();
            services.AddScoped<IMessageMapper, MessageMapper>();
            services.AddScoped<INotificationMapper, NotificationMapper>();
            services.AddScoped<ITaskMapper, TaskMapper>();
            services.AddScoped<IUniversityMapper, UniversityMapper>();
            services.AddScoped<IWallPostMapper, WallPostMapper>();

            return services;
        }
    }
}
