using Kampus.Models;
using Kampus.Persistence.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kampus.Application.Services
{
    public interface IWallPostService
    {
        Task<int> GetLastWallPostId();
        Task Delete(int wallPostId);
        Task<IReadOnlyList<WallPostModel>> GetAllPosts(int userId);
        Task<WallPostModel> WriteWallPost(int userId, int senderId, string content, List<FileModel> attachments);
        Task<WallPostCommentModel> WritePostComment(int userId, int postId, string text);
        Task<IReadOnlyList<WallPostCommentModel>> GetNewWallPostComments(int postId, int? postCommentId);
        Task<IReadOnlyList<WallPostModel>> GetLastWallPosts(int ownerId, int senderId, int lastPostId);
        Task<LikeResult> LikeWallPost(int userId, int postId);
    }
}
