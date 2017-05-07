using System;
using System.Collections.Generic;
using System.Linq;
using Kampus.Entities;
using Kampus.Models;
using Kampus.DAL.Enums;

namespace Kampus.DAL.Abstract.Repositories
{
    public interface IWallPostRepository: IRepository<WallPostModel>
    {
        List<WallPostModel> GetAllPosts(int userId);
        WallPostModel WriteWallPost(int userId, int senderId, string content, List<FileModel> attachments);
        WallPostCommentModel WritePostComment(int userId, int postId, string text);
        List<WallPostCommentModel> GetNewWallPostComments(int postId, int? postCommentId);
        List<WallPostModel> GetLastWallPosts(int ownerId, int senderId, int lastPostId);
        LikeResult LikeWallPost(int userId, int postId);
    }
}