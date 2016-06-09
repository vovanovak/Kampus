using System;
using System.Collections.Generic;
using System.Linq;
using Kampus.Entities;
using Kampus.Models;

namespace Kampus.DAL.Abstract
{
    public interface IWallPostRepository: IRepository<WallPostModel>
    {
        List<WallPostModel> GetAllPosts(int userid);
        WallPostModel WriteWallPost(int userid, int senderid, string content, List<FileModel> attachments);
        void WritePostComment(int userid, int postid, string text);
        List<WallPostCommentModel> GetNewWallPostComments(int postid, int? postcommentid);
        int LikeWallPost(int userid, int postid);
    }
}