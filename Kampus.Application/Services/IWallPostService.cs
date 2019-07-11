using Kampus.Models;
using Kampus.Persistence.Enums;
using System.Collections.Generic;

namespace Kampus.Application.Services
{
    public interface IWallPostService
    {
        List<WallPostModel> GetAllPosts(int userId);
        WallPostModel WriteWallPost(int userId, int senderId, string content, List<FileModel> attachments);
        WallPostCommentModel WritePostComment(int userId, int postId, string text);
        IReadOnlyList<WallPostCommentModel> GetNewWallPostComments(int postId, int? postCommentId);
        IReadOnlyList<WallPostModel> GetLastWallPosts(int ownerId, int senderId, int lastPostId);
        LikeResult LikeWallPost(int userId, int postId);
    }
}
