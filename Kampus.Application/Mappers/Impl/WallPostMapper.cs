using System.Collections.Generic;
using System.Linq;
using Kampus.Application.Extensions;
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
                Id = wallPost.WallPostId,
                Owner = wallPost.Owner.MapToUserShortModel(),
                Sender = wallPost.Sender.MapToUserShortModel(),
                Content = wallPost.Content,
                Likes = wallPost.Likes.Select(wpl => wpl.Liker.MapToUserShortModel()).ToList(),
                Comments = wallPost.Comments
                    .Select(wpc => new WallPostCommentModel()
                    {
                        Id = wpc.WallPostCommentId,
                        CreationTime = wpc.CreationTime,
                        Content = wpc.Content,
                        WallPostId = wpc.WallPostId,
                        Creator = wpc.Creator.MapToUserShortModel()
                    }).ToList(),
                Attachments = wallPost.Attachments.Select(wf => new FileModel()
                {
                    Id = wf.File.FileId,
                    RealFileName = wf.File.RealFileName,
                    FileName = wf.File.FileName
                }).ToList()
            };
        }

        public WallPost Map(WallPostModel entity, int ownerId, int senderId)
        {
            var wallPost = new WallPost { WallPostId = entity.Id, Content = entity.Content };

            wallPost.OwnerId = ownerId;
            wallPost.SenderId = senderId;

            if (entity.Attachments != null)
            {
                var files = entity.Attachments.Select(link => new File {RealFileName = link.RealFileName, FileName = link.FileName}).ToList();

                wallPost.Attachments = files.Select(f => new WallPostFile() { File = f, WallPost = wallPost }).ToList();
            }

            return wallPost;
        }
    }
}
