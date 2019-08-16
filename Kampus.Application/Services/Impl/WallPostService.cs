using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kampus.Application.Extensions;
using Kampus.Application.Mappers;
using Kampus.Models;
using Kampus.Persistence.Contexts;
using Kampus.Persistence.Entities.NotificationRelated;
using Kampus.Persistence.Entities.UserRelated;
using Kampus.Persistence.Entities.WallPostRelated;
using Kampus.Persistence.Enums;
using Microsoft.EntityFrameworkCore;

namespace Kampus.Application.Services.Impl
{
    internal class WallPostService : IWallPostService
    {
        private readonly KampusContext _context;
        private readonly IWallPostMapper _wallPostMapper;

        public WallPostService(KampusContext context, IWallPostMapper wallPostMapper)
        {
            _context = context;
            _wallPostMapper = wallPostMapper;
        }

        private IQueryable<WallPost> GetAllPostsWithRelatedEntities()
        {
            return _context.WallPosts
                .Include(p => p.Owner)
                .Include(p => p.Sender)
                .Include(p => p.Likes)
                    .ThenInclude(l => l.Liker)
                .Include(p => p.Comments)
                    .ThenInclude(c => c.Creator)
                .Include(p => p.Attachments);
        }

        public async Task<IReadOnlyList<WallPostModel>> GetAllPosts(int userId)
        {
            var models = await GetAllPostsWithRelatedEntities()
                .Where(p => p.OwnerId == userId)
                .OrderByDescending(p => p.WallPostId)
                .Select(wp => _wallPostMapper.Map(wp))
                .ToListAsync();

            return models;
        }

        public async Task<IReadOnlyList<WallPostModel>> GetLastWallPosts(int ownerId, int senderId, int lastPostId)
        {
            return await GetAllPostsWithRelatedEntities()
                .Where(p => p.OwnerId == ownerId && p.SenderId != senderId && p.WallPostId > lastPostId)
                .Select(wp => _wallPostMapper.Map(wp))
                .ToListAsync();
        }

        public async Task<IReadOnlyList<WallPostCommentModel>> GetNewWallPostComments(int postId, int? postCommentId)
        {
            var post = await GetAllPostsWithRelatedEntities().SingleOrDefaultAsync(p => p.WallPostId == postId);

            IReadOnlyList<WallPostComment> comments;

            if (postCommentId == null)
            {
                comments = post.Comments;
            }
            else
            {
                var comment = post.Comments.Single(c => c.WallPostCommentId == postCommentId);

                comments = post.Comments
                    .Where(p => comment.CreationTime.Ticks < p.CreationTime.Ticks &&
                                comment.WallPostCommentId != p.WallPostCommentId)
                    .ToList();
            }

            return comments.Select(MapToCommentModel).ToList();
        }

        private static WallPostCommentModel MapToCommentModel(WallPostComment c)
        {
            return new WallPostCommentModel
            {
                Id = c.WallPostCommentId,
                CreationTime = c.CreationTime,
                Content = c.Content,
                WallPostId = c.WallPostId,
                Creator = c.Creator.MapToUserShortModel()
            };
        }

        public async Task<LikeResult> LikeWallPost(int userId, int postId)
        {
            var user = await _context.Users.SingleAsync(u => u.UserId == userId);
            var wallPost = await _context.WallPosts.SingleAsync(u => u.WallPostId == postId);
            var hasWallPostLike = await _context.WallPostLikes.AnyAsync(l => l.LikerId == user.UserId && l.WallPostId == postId);

            if (hasWallPostLike)
            {
                var likes = await _context.WallPostLikes
                    .Where(l => l.LikerId == user.UserId && l.WallPostId == postId)
                    .ToListAsync();

                _context.WallPostLikes.RemoveRange(likes);
                await _context.SaveChangesAsync();

                return LikeResult.Unliked;
            }

            var like = new WallPostLike
            {
                WallPost = wallPost,
                WallPostId = wallPost.WallPostId,
                Liker = user,
                LikerId = user.UserId
            };

            _context.WallPostLikes.Add(like);

            if (wallPost.Owner.UserId != user.UserId)
            {
                var notification = Notification.From(DateTime.Now, NotificationType.WallPostWritten,
                    user, wallPost.Owner, "@" + user.Username + " оцінив запис на вашій стіні", "/Home/Post/" + postId);
                _context.Notifications.Add(notification);
            }

            await _context.SaveChangesAsync();

            return LikeResult.Liked;
        }

        public async Task<WallPostCommentModel> WritePostComment(int userId, int postId, string text)
        {
            var comment = new WallPostComment
            {
                Content = text,
                WallPostId = postId,
                WallPost = await _context.WallPosts.SingleAsync(p => p.WallPostId == postId),
                Creator = await _context.Users.SingleAsync(u => u.UserId == userId),
                CreationTime = DateTime.Now
            };


            if (comment.WallPost.OwnerId != userId)
            {
                var notification = Notification.From(DateTime.Now, NotificationType.WallPostWritten,
                    comment.Creator, comment.WallPost.Owner, "/Home/Post/" + postId, " коментував запис на вашій стіні");
                _context.Notifications.Add(notification);
            }

            _context.WallPostComments.Add(comment);
            await _context.SaveChangesAsync();

            return await _context.WallPostComments
                .Where(c => c.WallPostId == postId && c.CreationTime == comment.CreationTime)
                .Select(dbComment => new WallPostCommentModel
                {
                    Id = dbComment.WallPostCommentId,
                    CreationTime = dbComment.CreationTime,
                    Content = dbComment.Content,
                    WallPostId = dbComment.WallPostId,
                    Creator = new UserShortModel(dbComment.Creator.UserId, dbComment.Creator.Username,
                        dbComment.Creator.Avatar)
                })
                .OrderByDescending(c => c.Id)
                .FirstAsync();
        }

        public async Task<WallPostModel> WriteWallPost(int userId, int senderId, string content, List<FileModel> attachments)
        {
            var sender = await _context.Users.SingleAsync(u => u.UserId == senderId);
            var user = await _context.Users.SingleAsync(u => u.UserId == userId);

            if (attachments == null)
                attachments = new List<FileModel>();

            var model = new WallPostModel
            {
                Content = content,
                Sender = new UserShortModel(sender.UserId, sender.Avatar, sender.Username),
                Owner = new UserShortModel(user.UserId, user.Avatar, user.Username),
                Attachments = attachments.Select(f => new FileModel
                {
                    Id = f.Id,
                    RealFileName = f.RealFileName,
                    FileName = f.FileName
                }).ToList(),
                Comments = new List<WallPostCommentModel>(),
                Likes = new List<UserShortModel>()
            };

            var dbEntity = _wallPostMapper.Map(model, user.UserId, sender.UserId);
            _context.Files.AddRange(dbEntity.Attachments.Select(wf => wf.File));
            _context.WallPosts.Add(dbEntity);

            if (userId != senderId)
            {
                var notification = new Notification(DateTime.Now, NotificationType.WallPostWritten,
                    sender, user, " написав на вашій стіні: \"" + content + "\"", "/Home");

                _context.Notifications.Add(notification);
            }

            await _context.SaveChangesAsync();
            return model;
        }

        public async Task<int> GetLastWallPostId()
        {
            return (await _context.WallPosts.LastAsync()).WallPostId;
        }

        public async Task Delete(int wallPostId)
        {
            var wallPost = await _context.WallPosts.SingleAsync(p => p.WallPostId == wallPostId);
            _context.WallPosts.Remove(wallPost);
            await _context.SaveChangesAsync();
        }
    }
}
