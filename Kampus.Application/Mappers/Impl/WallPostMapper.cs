using System;
using System.Collections.Generic;
using System.Linq;
using Kampus.Models;
using Kampus.Persistence.Entities.AttachmentsRelated;
using Kampus.Persistence.Entities.WallPostRelated;

namespace Kampus.Application.Mappers.Impl
{
    internal class WallPostMapper : IWallPostMapper
    {
        public WallPostModel Map(WallPost wallPost)
        {
            return new WallPostModel()
            {
                Id = wallPost.Id,
                Owner = new UserShortModel() { Id = wallPost.Owner.Id, Username = wallPost.Owner.Username, Avatar = wallPost.Owner.Avatar },
                Sender = new UserShortModel() { Id = wallPost.Sender.Id, Username = wallPost.Sender.Username, Avatar = wallPost.Sender.Avatar },
                Content = wallPost.Content,
                Likes = wallPost.Likes.Select(p2 => new UserShortModel() { Id = p2.Liker.Id, Username = p2.Liker.Username, Avatar = p2.Liker.Avatar }).ToList(),

                Comments = wallPost.Comments
                    .Select(p => new WallPostCommentModel()
                    {
                        Id = p.Id,
                        CreationTime = p.CreationTime,
                        Content = p.Content,
                        WallPostId = p.WallPostId,
                        Creator = new UserShortModel() { Id = p.Creator.Id, Username = p.Creator.Username, Avatar = p.Creator.Avatar }
                    }).ToList(),
                Attachments = wallPost.Attachments.Select(f => new FileModel()
                {
                    Id = f.Id,
                    RealFileName = f.RealFileName,
                    FileName = f.FileName
                }).ToList()
            };
        }

        public WallPost Map(WallPostModel entity)
        {
            var dbEntity = new WallPost();

            dbEntity.Id = entity.Id;
            dbEntity.Content = entity.Content;
            dbEntity.OwnerId = dbEntity.Owner.Id;
            dbEntity.SenderId = dbEntity.Sender.Id;

            if (entity.Attachments != null)
            {
                var files = new List<File>();
                foreach (var link in entity.Attachments)
                {
                    File file = new File();
                    file.RealFileName = link.RealFileName;
                    file.FileName = link.FileName;
                    files.Add(file);
                }
                dbEntity.Attachments = files;
            }

            return dbEntity;
        }
    }
}
