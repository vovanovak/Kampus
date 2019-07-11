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
            return _context.UserPosts
                .Include(p => p.Likes)
                    .ThenInclude((WallPostLike l) => l.Liker)
                .Include(p => p.Comments)
                    .ThenInclude((WallPostComment c) => c.Creator)
                .Include(p => p.Attachments);
        }

        public List<WallPostModel> GetAllPosts(int userId)
        {
            List<WallPostModel> models = GetAllPostsWithRelatedEntities()
                .Where(p => p.OwnerId == userId)
                .OrderByDescending(p => p.Id)
                .Select(_wallPostMapper.Map)
                .ToList();

            return models;
        }

        public IReadOnlyList<WallPostModel> GetLastWallPosts(int ownerId, int senderId, int lastPostId)
        {
            return GetAllPostsWithRelatedEntities()
                .Where(p => p.OwnerId == ownerId && p.SenderId != senderId && p.Id > lastPostId)
                .Select(_wallPostMapper.Map)
                .ToList();
        }

        public IReadOnlyList<WallPostCommentModel> GetNewWallPostComments(int postId, int? postCommentId)
        {
            WallPost post = _context.UserPosts.First(p => p.Id == postId);

            if (postCommentId == null)
            {
                try
                {
                    if (post.Comments.Any())
                    {
                        List<WallPostCommentModel> comments = post.Comments.Select(c => new WallPostCommentModel()
                        {
                            Id = c.Id,
                            CreationTime = c.CreationTime,
                            Content = c.Content,
                            WallPostId = c.WallPostId,
                            Creator = new UserShortModel() { Id = c.Creator.Id, Username = c.Creator.Username, Avatar = c.Creator.Avatar }
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

                WallPostComment comment = post.Comments.First(c => c.Id == postCommentId);

                List<WallPostComment> comments =
                    post.Comments.Where(p => comment.CreationTime.Ticks < p.CreationTime.Ticks && comment.Id != p.Id).ToList();

                if (comments.Any())
                {
                    return comments.Select(c => new WallPostCommentModel()
                    {
                        Id = c.Id,
                        CreationTime = c.CreationTime,
                        Content = c.Content,
                        WallPostId = c.WallPostId,
                        Creator = UserShortModel.From(c.Creator.Id, c.Creator.Username, c.Creator.Avatar)
                    }).ToList();
                }
                else
                {
                    return new List<WallPostCommentModel>();
                }
            }
        }

        public LikeResult LikeWallPost(int userId, int postId)
        {
            User user = _context.Users.First(u => u.Id == userId);

            WallPost post = _context.UserPosts.First(p => p.Id == postId);

            if (_context.UserPostLikes.Any(l => l.LikerId == user.Id && l.WallPostId == postId))
            {
                List<WallPostLike> likes =
                    _context.UserPostLikes.Where(l => l.LikerId == user.Id && l.WallPostId == postId).ToList();
                _context.UserPostLikes.RemoveRange(likes);
                _context.SaveChanges();
                return LikeResult.Unliked;
            }
            else
            {
                WallPostLike like = new WallPostLike();

                like.WallPost = post;
                like.WallPostId = post.Id;

                like.Liker = user;
                like.LikerId = user.Id;

                _context.UserPostLikes.Add(like);

                if (post.Owner.Id != user.Id)
                {
                    Notification notification = Notification.From(DateTime.Now, NotificationType.WallPostWritten,
                        user, post.Owner, "@" + user.Username + " оцінив запис на вашій стіні", "/Home/Post/" + postId);
                    _context.Notifications.Add(notification);
                }
                _context.SaveChanges();
                return LikeResult.Liked;
            }
        }

        public WallPostCommentModel WritePostComment(int userId, int postId, string text)
        {
            WallPostComment comment = new WallPostComment();

            comment.Content = text;
            comment.WallPostId = postId;
            comment.WallPost = _context.UserPosts.First(p => p.Id == postId);
            comment.Creator = _context.Users.First(u => u.Id == userId);
            comment.CreationTime = DateTime.Now;

            if (comment.WallPost.OwnerId != userId)
            {
                Notification notification = Notification.From(DateTime.Now, NotificationType.WallPostWritten,
                    comment.Creator, comment.WallPost.Owner, "/Home/Post/" + postId, " коментував запис на вашій стіні");
                _context.Notifications.Add(notification);
            }

            _context.UserPostComments.Add(comment);
            _context.SaveChanges();

            return _context.UserPostComments.Where(c => c.WallPostId == postId &&
                   c.CreationTime == comment.CreationTime).Select(dbComment => new WallPostCommentModel()
                   {
                       Id = dbComment.Id,
                       CreationTime = dbComment.CreationTime,
                       Content = dbComment.Content,
                       WallPostId = dbComment.WallPostId,
                       Creator = new UserShortModel() { Id = dbComment.Creator.Id, Username = dbComment.Creator.Username, Avatar = dbComment.Creator.Avatar }
                   }).OrderByDescending(c => c.Id).Take(1).Single();
        }

        public WallPostModel WriteWallPost(int userId, int senderId, string content, List<FileModel> attachments)
        {
            User sender = _context.Users.First(u => u.Id == senderId);
            User user = _context.Users.First(u => u.Id == userId);

            if (attachments == null)
                attachments = new List<FileModel>();

            WallPostModel model = new WallPostModel()
            {
                Content = content,
                Sender = new UserShortModel() { Id = sender.Id, Avatar = sender.Avatar, Username = sender.Username },
                Owner = new UserShortModel() { Id = user.Id, Avatar = user.Avatar, Username = user.Username },
                Attachments = attachments.Select(f => new FileModel()
                {
                    Id = f.Id,
                    RealFileName = f.RealFileName,
                    FileName = f.FileName
                }).ToList(),
                Comments = new List<WallPostCommentModel>(),
                Likes = new List<UserShortModel>()
            };

            var dbEntity = _wallPostMapper.Map(model);
            _context.Files.AddRange(dbEntity.Attachments);
            _context.UserPosts.Add(dbEntity);

            _context.SaveChanges();

            if (userId != senderId)
            {
                Notification notification = Notification.From(DateTime.Now, NotificationType.WallPostWritten,
                    sender, user, " написав на вашій стіні: \"" + content + "\"", "/Home");

                _context.Notifications.Add(notification);
            }

            _context.SaveChanges();
            return model;
        }

        public int GetLastWallPostId()
        {
            return _context.UserPosts.Last().Id;
        }

        public void Delete(int wallPostId)
        {
            var wallPost = _context.UserPosts.Single(p => p.Id == wallPostId);
            _context.UserPosts.Remove(wallPost);
            _context.SaveChanges();
        }
    }
}
