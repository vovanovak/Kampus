using System;
using System.Reflection;
using Autofac;
using Kampus.DAL.Abstract;
using Kampus.DAL.Concrete;

namespace Kampus.Container
{
    public class Autofac
    {
        public static IContainer Container { get; set; }

        static Autofac()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<UserRepositoryBase>().As<IUserRepository>();
            builder.RegisterType<CityRepositoryBase>().As<ICityRepository>(); 
            builder.RegisterType<MessageRepositoryBase>().As<IMessageRepository>();
            builder.RegisterType<TaskRepositoryBase>().As<ITaskRepository>();
            builder.RegisterType<UniversityRepositoryBase>().As<IUniversityRepository>();
            builder.RegisterType<WallPostRepositoryBase>().As<IWallPostRepository>();
            builder.RegisterType<NotificationRepositoryBase>().As<INotificationRepository>();

            Container = builder.Build();
        }
    }
}