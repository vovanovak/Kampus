using Kampus.Models;
using Kampus.Persistence.Entities.UserRelated;

namespace Kampus.Application.Mappers
{
    public interface IUserMapper
    {
        UserModel Map(User user);
    }
}
