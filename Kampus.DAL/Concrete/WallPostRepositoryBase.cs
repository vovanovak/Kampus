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

namespace Kampus.DAL.Concrete
{
    public class WallPostRepositoryBase: RepositoryBase<WallPostModel, WallPost>, IWallPostRepository
    {
        protected override DbSet<WallPost> GetTable()
        {
            return ctx.UserPosts;
        }

        protected override Expression<Func<WallPost, WallPostModel>> GetConverter()
        {
            return w => new WallPostModel()
            {
                Id = w.Id,
                User = new UserShortModel() { Id = w.User.Id, Username = w.User.Username, Avatar = w.User.Avatar },
                Sender = new UserShortModel() { Id = w.Sender.Id, Username = w.Sender.Username, Avatar = w.Sender.Avatar },
                Content = w.Content,
                Likes = ctx.UserPostLikes.Where(p => p.WallPostId == w.Id).
                    Select(p1 => p1.User).Select(p2 => new UserShortModel() { Id = p2.Id, Username = p2.Username, Avatar = p2.Avatar }).ToList(),

                Comments = ctx.UserPostComments.Where(c => c.WallPostId == w.Id).
                    Select(p => new WallPostCommentModel()
                    {
                        Id = p.Id,
                        CreationTime = p.CreationTime,
                        Content = p.Content,
                        WallPostId = p.WallPostId,
                        User = new UserShortModel() { Id = p.User.Id, Username = p.User.Username, Avatar = p.User.Avatar }
                    }).ToList(),
                Attachments = w.Attachments.Select(f => new FileModel() { 
                    Id = f.Id,
                    RealFileName = f.RealFileName,
                    FileName = f.FileName, 
                    Extension = f.FileName.Substring(f.FileName.IndexOf(".")),
                    IsImage = f.FileName.Substring(f.FileName.IndexOf(".")) == ".jpg"
                }).ToList()
            };
        }

        protected override void UpdateEntry(WallPost dbEntity, WallPostModel entity)
        {
            dbEntity.Id = entity.Id;
            dbEntity.Content = entity.Content;
            dbEntity.User = ctx.Users.First(u => u.Id == entity.User.Id);
            dbEntity.Sender = ctx.Users.First(u => u.Id == entity.Sender.Id);
            dbEntity.UserId = dbEntity.User.Id;
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

        public List<WallPostModel> GetAllPosts(int userid)
        {
            List<WallPostModel> models = ctx.UserPosts.Where(p => p.UserId == userid).Select(GetConverter()).OrderByDescending(p => p.Id).ToList();
            return models;
        }

        public WallPostModel WriteWallPost(int userid, int senderid, string content, List<FileModel> attachments)
        {
            User sender = ctx.Users.First(u => u.Id == senderid);
            User user = ctx.Users.First(u => u.Id == userid);

            if (attachments == null)
                attachments = new List<FileModel>();

            WallPostModel model = new WallPostModel()
            {
                Content = content,
                Sender = new UserShortModel() { Id = sender.Id, Avatar = sender.Avatar, Username = sender.Username },
                User = new UserShortModel() { Id = user.Id, Avatar = user.Avatar, Username = user.Username },
                Attachments = attachments.Select(f => new FileModel()
                {
                    Id = f.Id,
                    RealFileName = f.RealFileName,
                    FileName = f.FileName,
                    Extension = f.FileName.Substring(f.FileName.IndexOf(".")),
                    IsImage = f.FileName.Substring(f.FileName.IndexOf(".")) == ".jpg"
                }).ToList(),
                Comments = new List<WallPostCommentModel>(),
                Likes = new List<UserShortModel>()
            };

            

            Save(model);

            ctx.SaveChanges();

            if (userid != senderid)
            {
                Notification notification = new Notification();
                notification.Date = DateTime.Now;
                notification.Type = NotificationType.WallPostWritten;
                notification.User = user;
                notification.UserId = user.Id;
                notification.SenderId = senderid;
                notification.Sender = sender;
                notification.Message = " написав на вашій стіні: \"" + content + "\"";
                notification.Link = "/Home";

                ctx.Notifications.Add(notification);
            }

            ctx.SaveChanges();

            
            return model;
        }

        public void WritePostComment(int userid, int postid, string text)
        {
            WallPostComment comment = new WallPostComment();

            comment.Content = text;
            comment.WallPostId = postid;
            comment.WallPost = ctx.UserPosts.First(p => p.Id == postid);
            comment.User = ctx.Users.First(u => u.Id == userid);
            comment.CreationTime = DateTime.Now;

            if (comment.WallPost.UserId != userid)
            {
                Notification notification = new Notification();
                notification.Date = DateTime.Now;
                notification.Type = NotificationType.WallPostWritten;
                notification.User = comment.WallPost.User;
                notification.UserId = comment.WallPost.User.Id;
                notification.SenderId = userid;
                notification.Sender = comment.User;
                notification.Message =
                    " коментував запис на вашій стіні";
                notification.Link = "/Home/Post/" + postid;

                ctx.Notifications.Add(notification);
            }

            ctx.UserPostComments.Add(comment);
            ctx.SaveChanges();
        }

        public List<WallPostCommentModel> GetNewWallPostComments(int postid, int? postcommentid)
        {
            WallPost post = ctx.UserPosts.First(p => p.Id == postid);

            if (postcommentid == null)
            {
                if (post.Comments.Any())
                {
                    List<WallPostCommentModel> comments = post.Comments.Select(c => new WallPostCommentModel()
                    {
                        Id = c.Id,
                        CreationTime = c.CreationTime,
                        Content = c.Content,
                        WallPostId = c.WallPostId,
                        User = new UserShortModel() { Id = c.User.Id, Username = c.User.Username, Avatar = c.User.Avatar}
                    }).ToList();

                    return comments;
                }
                else
                {
                    return new List<WallPostCommentModel>();
                }
            }
            else
            {

                WallPostComment comment = post.Comments.First(c => c.Id == postcommentid);

                List<WallPostComment> comments =
                    post.Comments.Where(p => comment.CreationTime.Ticks < p.CreationTime.Ticks).ToList();

                if (comments.Any())
                {
                    return comments.Select(c => new WallPostCommentModel()
                    {
                        Id = c.Id,
                        CreationTime = c.CreationTime,
                        Content = c.Content,
                        WallPostId = c.WallPostId,
                        User = new UserShortModel() {Id = c.User.Id, Username = c.User.Username, Avatar = c.User.Avatar}
                    }).ToList();
                }
                else
                {
                    return new List<WallPostCommentModel>();
                }
            }
        }

      


        public int LikeWallPost(int userid, int postid)
        {
            User user = ctx.Users.First(u => u.Id == userid);

            WallPost post = ctx.UserPosts.First(p => p.Id == postid);

            int res = 0;

            if (ctx.UserPostLikes.Any(l => l.UserId == user.Id && l.WallPostId == postid))
            {
                List<WallPostLike> likes =
                    ctx.UserPostLikes.Where(l => l.UserId == user.Id && l.WallPostId == postid).ToList();
                ctx.UserPostLikes.RemoveRange(likes);

            }
            else
            {
                WallPostLike like = new WallPostLike();

                like.WallPost = post;
                like.WallPostId = post.Id;

                like.User = user;
                like.UserId = user.Id;

                ctx.UserPostLikes.Add(like);

                if (post.User.Id != user.Id)
                {
                    Notification notification = new Notification();
                    notification.Date = DateTime.Now;
                    notification.Type = NotificationType.WallPostWritten;
                    notification.User = post.User;
                    notification.UserId = post.User.Id;
                    notification.SenderId = userid;
                    notification.Sender = user;
                    notification.Message = "@" + user.Username + " оцінив запис на вашій стіні";
                    notification.Link = "/Home/Post/" + postid;

                    ctx.Notifications.Add(notification);
                }

                res = 1;
            }

            ctx.SaveChanges();

            return res;
        }

    }

     
}