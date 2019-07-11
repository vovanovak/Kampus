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
            
        }

        public WallPostModel WriteWallPost(int userId, int senderId, string content, List<FileModel> attachments)
        {
            
        }

        public WallPostCommentModel WritePostComment(int userId, int postId, string text)
        {
            
        }

        public List<WallPostCommentModel> GetNewWallPostComments(int postId, int? postCommentId)
        {
            
        }

      


        public LikeResult LikeWallPost(int userId, int postId)
        {
            
        }

        public List<WallPostModel> GetLastWallPosts(int ownerId, int senderId, int lastPostId)
        {
            
        }
    }
}