using Kampus.DAL.Abstract;
using Kampus.DAL.Abstract.Repositories;
using Kampus.DAL.Concrete;
using Kampus.DAL.Concrete.Repositories;
using Kampus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Kampus.DAL
{
    internal class UnitOfWork : IUnitOfWork
    {
        private KampusContext _context;

        public ICityRepository Cities { get; set; }
        public IMessageRepository Messages { get; set; }
        public INotificationRepository Notifications { get; set; }
        public ITaskRepository Tasks { get; set; }
        public IUniversityRepository Universities { get; set; }
        public IUserRepository Users { get; set; }
        public IWallPostRepository WallPosts { get; set; }

        public UnitOfWork()
        {
            _context = new KampusContext();

            Cities = new CityRepositoryBase(_context);
            Messages = new MessageRepositoryBase(_context);
            Notifications = new NotificationRepositoryBase(_context);
            Tasks = new TaskRepositoryBase(_context);
            Universities = new UniversityRepositoryBase(_context);
            Users = new UserRepositoryBase(_context);
            WallPosts = new WallPostRepositoryBase(_context);
        }
    }
}

