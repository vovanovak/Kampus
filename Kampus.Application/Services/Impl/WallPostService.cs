using System;
using System.Collections.Generic;
using System.Linq;
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
                .Include(p => p.Likes)
                    .ThenInclude(l => l.Liker)
                .Include(p => p.Comments)
                    .ThenInclude(c => c.Creator)
                .Include(p => p.Attachments);
        }

        public List<WallPostModel> GetAllPosts(int userId)
        {
            List<WallPostModel> models = GetAllPostsWithRelatedEntities()
                .Where(p => p.OwnerId == userId)
                .OrderByDescending(p => p.WallPostId)
                .Select(_wallPostMapper.Map)
                .ToList();

            return models;
        }

        public IReadOnlyList<WallPostModel> GetLastWallPosts(int ownerId, int senderId, int lastPostId)
        {
            return GetAllPostsWithRelatedEntities()
                .Where(p => p.OwnerId == ownerId && p.SenderId != senderId && p.WallPostId > lastPostId)
                .Select(_wallPostMapper.Map)
                .ToList();
        }

        public IReadOnlyList<WallPostCommentModel> GetNewWallPostComments(int postId, int? postCommentId)
        {
            var post = _context.WallPosts.First(p => p.WallPostId == postId);

            if (postCommentId == null)
            {
                try
                {
                    if (post.Comments.Any())
                    {
                        List<WallPostCommentModel> comments = post.Comments.Select(c => new WallPostCommentModel()
                        {
                            Id = c.WallPostId,
                            CreationTime = c.CreationTime,
                            Content = c.Content,
                            WallPostId = c.WallPostId,
                            Creator = new UserShortModel() { Id = c.Creator.UserId, Username = c.Creator.Username, Avatar = c.Creator.Avatar }
                        }).ToList();

                        return comments;
                    }
                    else
                    {
                        return new List<WallPostCommentModel>();
                    }
                }
                catch (Exception e)
                {
                    return new List<WallPostCommentModel>();
                }
            }
            else
            {

                var comment = post.Comments.First(c => c.WallPostCommentId == postCommentId);

                List<WallPostComment> comments =
                    post.Comments.Where(p => comment.CreationTime.Ticks < p.CreationTime.Ticks && comment.WallPostCommentId != p.WallPostCommentId).ToList();

                if (comments.Any())
                {
                    return comments.Select(c => new WallPostCommentModel
                    {
                        Id = c.WallPostCommentId,
                        CreationTime = c.CreationTime,
                        Content = c.Content,
                        WallPostId = c.WallPostId,
                        Creator = new UserShortModel(c.Creator.UserId, c.Creator.Username, c.Creator.Avatar)
                    }).ToList();
                }

                return new List<WallPostCommentModel>();
            }
        }

        public LikeResult LikeWallPost(int userId, int postId)
        {
            var user = _context.Users.First(u => u.UserId == userId);

            var post = _context.WallPosts.First(p => p.WallPostId == postId);

            if (_context.WallPostLikes.Any(l => l.LikerId == user.UserId && l.WallPostId == postId))
            {
                List<WallPostLike> likes =
                    _context.WallPostLikes.Where(l => l.LikerId == user.UserId && l.WallPostId == postId).ToList();
                _context.WallPostLikes.RemoveRange(likes);
                _context.SaveChanges();
                return LikeResult.Unliked;
            }

            var like = new WallPostLike
            {
                WallPost = post,
                WallPostId = post.WallPostId,
                Liker = user,
                LikerId = user.UserId
            };

            _context.WallPostLikes.Add(like);

            if (post.Owner.UserId != user.UserId)
            {
                var notification = Notification.From(DateTime.Now, NotificationType.WallPostWritten,
                    user, post.Owner, "@" + user.Username + " оцінив запис на вашій стіні", "/Home/Post/" + postId);
                _context.Notifications.Add(notification);
            }

            _context.SaveChanges();

            return LikeResult.Liked;
        }

        public WallPostCommentModel WritePostComment(int userId, int postId, string text)
        {
            WallPostComment comment = new WallPostComment();

            comment.Content = text;
            comment.WallPostId = postId;
            comment.WallPost = _context.WallPosts.First(p => p.WallPostId == postId);
            comment.Creator = _context.Users.First(u => u.UserId == userId);
            comment.CreationTime = DateTime.Now;

            if (comment.WallPost.OwnerId != userId)
            {
                var notification = Notification.From(DateTime.Now, NotificationType.WallPostWritten,
                    comment.Creator, comment.WallPost.Owner, "/Home/Post/" + postId, " коментував запис на вашій стіні");
                _context.Notifications.Add(notification);
            }

            _context.WallPostComments.Add(comment);
            _context.SaveChanges();

            return _context.WallPostComments
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
                .First();
        }

        public WallPostModel WriteWallPost(int userId, int senderId, string content, List<FileModel> attachments)
        {
            var sender = _context.Users.First(u => u.UserId == senderId);
            var user = _context.Users.First(u => u.UserId == userId);

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

            var dbEntity = _wallPostMapper.Map(model);
            _context.Files.AddRange(dbEntity.Attachments.Select(wf => wf.File));
            _context.WallPosts.Add(dbEntity);

            _context.SaveChanges();

            if (userId != senderId)
            {
                var notification = new Notification(DateTime.Now, NotificationType.WallPostWritten,
                    sender, user, " написав на вашій стіні: \"" + content + "\"", "/Home");

                _context.Notifications.Add(notification);
            }

            _context.SaveChanges();
            return model;
        }

        public int GetLastWallPostId()
        {
            return _context.WallPosts.Last().WallPostId;
        }

        public void Delete(int wallPostId)
        {
            var wallPost = _context.WallPosts.Single(p => p.WallPostId == wallPostId);
            _context.WallPosts.Remove(wallPost);
            _context.SaveChanges();
        }
    }
}
