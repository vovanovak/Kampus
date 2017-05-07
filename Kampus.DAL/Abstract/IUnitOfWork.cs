using Kampus.DAL.Abstract.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kampus.DAL.Abstract
{
    public interface IUnitOfWork
    {
        ICityRepository Cities { get; set; }
        IMessageRepository Messages { get; set; }
        INotificationRepository Notifications { get; set; }
        ITaskRepository Tasks { get; set; }
        IUniversityRepository Universities { get; set; }
        IUserRepository Users { get; set; }
        IWallPostRepository WallPosts { get; set; }
    }
}
