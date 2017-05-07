using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Runtime.CompilerServices;
using Kampus.DAL.Abstract;
using Kampus.Entities;
using Kampus.Models;
using Kampus.DAL.Abstract.Repositories;
using Kampus.DAL.Enums;

namespace Kampus.DAL.Concrete.Repositories
{
    internal class WallPostRepositoryBase: RepositoryBase<WallPostModel, WallPost>, IWallPostRepository
    {
        public WallPostRepositoryBase(KampusContext context) : base(context)
        {
        }

        protected override DbSet<WallPost> GetTable()
        {
            return ctx.UserPosts;
        }

        protected override Expression<Func<WallPost, WallPostModel>> GetConverter()
        {
            return w => new WallPostModel()
            {
                Id = w.Id,
                Owner = new UserShortModel() { Id = w.Owner.Id, Username = w.Owner.Username, Avatar = w.Owner.Avatar },
                Sender = new UserShortModel() { Id = w.Sender.Id, Username = w.Sender.Username, Avatar = w.Sender.Avatar },
                Content = w.Content,
                Likes = ctx.UserPostLikes.Where(p => p.WallPostId == w.Id).
                    Select(p1 => p1.Liker).Select(p2 => new UserShortModel() { Id = p2.Id, Username = p2.Username, Avatar = p2.Avatar }).ToList(),

                Comments = ctx.UserPostComments.Where(c => c.WallPostId == w.Id).
                    Select(p => new WallPostCommentModel()
                    {
                        Id = p.Id,
                        CreationTime = p.CreationTime,
                        Content = p.Content,
                        WallPostId = p.WallPostId,
                        Creator = new UserShortModel() { Id = p.Creator.Id, Username = p.Creator.Username, Avatar = p.Creator.Avatar }
                    }).ToList(),
                Attachments = w.Attachments.Select(f => new FileModel() { 
                    Id = f.Id,
                    RealFileName = f.RealFileName,
                    FileName = f.FileName
                }).ToList()
            };
        }

        protected override void UpdateEntry(WallPost dbEntity, WallPostModel entity)
        {
            dbEntity.Id = entity.Id;
            dbEntity.Content = entity.Content;
            dbEntity.Owner = ctx.Users.First(u => u.Id == entity.Owner.Id);
            dbEntity.Sender = ctx.Users.First(u => u.Id == entity.Sender.Id);
            dbEntity.OwnerId = dbEntity.Owner.Id;
            dbEntity.SenderId = dbEntity.Sender.Id;

            if (entity.Attachments != null)
            {
                List<File> files = new List<File>();
                foreach (var link in entity.Attachments)
                {
                    if (!ctx.Files.Any(f => f.RealFileName == link.RealFileName))
                    {
                        File file = new File();
                        file.RealFileName = link.RealFileName;
                        file.FileName = link.FileName;

                        files.Add(file);
                        ctx.Files.Add(file);
                    }
                    else
                    {
                        files.Add(ctx.Files.First(f => f.RealFileName == link.RealFileName));
                    }
                }
                dbEntity.Attachments = files;
            }
        }

        public List<WallPostModel> GetAllPosts(int userId)
        {
            List<WallPostModel> models = ctx.UserPosts.Where(p => p.OwnerId == userId).
                Select(GetConverter()).OrderByDescending(p => p.Id).ToList();
            return models;
        }

        public WallPostModel WriteWallPost(int userId, int senderId, string content, List<FileModel> attachments)
        {
            User sender = ctx.Users.First(u => u.Id == senderId);
            User user = ctx.Users.First(u => u.Id == userId);

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

            Save(model);

            ctx.SaveChanges();

            if (userId != senderId)
            {
                Notification notification = Notification.From(DateTime.Now, NotificationType.WallPostWritten, 
                    sender, user, " написав на вашій стіні: \"" + content + "\"", "/Home");
                
                ctx.Notifications.Add(notification);
            }

            ctx.SaveChanges();
            return model;
        }

        public WallPostCommentModel WritePostComment(int userId, int postId, string text)
        {
            WallPostComment comment = new WallPostComment();

            comment.Content = text;
            comment.WallPostId = postId;
            comment.WallPost = ctx.UserPosts.First(p => p.Id == postId);
            comment.Creator = ctx.Users.First(u => u.Id == userId);
            comment.CreationTime = DateTime.Now;

            if (comment.WallPost.OwnerId != userId)
            {
                Notification notification = Notification.From(DateTime.Now, NotificationType.WallPostWritten, 
                    comment.Creator, comment.WallPost.Owner, "/Home/Post/" + postId, " коментував запис на вашій стіні");
                ctx.Notifications.Add(notification);
            }

            ctx.UserPostComments.Add(comment);
            ctx.SaveChanges();

            return ctx.UserPostComments.Where(c => c.WallPostId == postId && 
                   c.CreationTime == comment.CreationTime).Select(dbComment => new WallPostCommentModel()
                   {
                       Id = dbComment.Id,
                       CreationTime = dbComment.CreationTime,
                       Content = dbComment.Content,
                       WallPostId = dbComment.WallPostId,
                       Creator = new UserShortModel() { Id = dbComment.Creator.Id, Username = dbComment.Creator.Username, Avatar = dbComment.Creator.Avatar }
                   }).OrderByDescending(c => c.Id).Take(1).Single();
        }

        public List<WallPostCommentModel> GetNewWallPostComments(int postId, int? postCommentId)
        {
            WallPost post = ctx.UserPosts.First(p => p.Id == postId);

            if (postCommentId == null)
            {
                try { 
                    if (post.Comments.Any())
                    {
                        List<WallPostCommentModel> comments = post.Comments.Select(c => new WallPostCommentModel()
                        {
                            Id = c.Id,
                            CreationTime = c.CreationTime,
                            Content = c.Content,
                            WallPostId = c.WallPostId,
                            Creator = new UserShortModel() { Id = c.Creator.Id, Username = c.Creator.Username, Avatar = c.Creator.Avatar}
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
            User user = ctx.Users.First(u => u.Id == userId);

            WallPost post = ctx.UserPosts.First(p => p.Id == postId);
            
            if (ctx.UserPostLikes.Any(l => l.LikerId == user.Id && l.WallPostId == postId))
            {
                List<WallPostLike> likes =
                    ctx.UserPostLikes.Where(l => l.LikerId == user.Id && l.WallPostId == postId).ToList();
                ctx.UserPostLikes.RemoveRange(likes);
                ctx.SaveChanges();
                return LikeResult.Unliked;
            }
            else
            {
                WallPostLike like = new WallPostLike();

                like.WallPost = post;
                like.WallPostId = post.Id;

                like.Liker = user;
                like.LikerId = user.Id;

                ctx.UserPostLikes.Add(like);
                

                if (post.Owner.Id != user.Id)
                {
                    Notification notification = Notification.From(DateTime.Now, NotificationType.WallPostWritten,
                        user, post.Owner, "@" + user.Username + " оцінив запис на вашій стіні", "/Home/Post/" + postId);
                    ctx.Notifications.Add(notification);
                }
                ctx.SaveChanges();
                return LikeResult.Liked;
            }
        }

        public List<WallPostModel> GetLastWallPosts(int ownerId, int senderId, int lastPostId)
        {
            return ctx.UserPosts.Where(p => p.OwnerId == ownerId && p.SenderId != senderId && p.Id > lastPostId).
                Select(GetConverter()).ToList();
        }
    }
}