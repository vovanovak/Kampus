using Kampus.Models;
using Kampus.Persistence.Entities.WallPostRelated;

namespace Kampus.Application.Mappers
{
    public interface IWallPostMapper
    {
        WallPostModel Map(WallPost wallPost);
        WallPost Map(WallPostModel wallPostModel);
    }
}
