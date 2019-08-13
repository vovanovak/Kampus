using System.Linq;
using Kampus.Application.Extensions;
using Kampus.Models;
using Kampus.Persistence.Entities.TaskRelated;
using Kampus.Persistence.Entities.UserRelated;

namespace Kampus.Application.Mappers.Impl
{
    internal class TaskMapper : ITaskMapper
    {
        public TaskModel Map(TaskEntry taskEntry)
        {
            return new TaskModel()
            {
                Id = taskEntry.TaskId,
                Category = taskEntry.TaskCat.TaskCategoryId,
                CategoryName = taskEntry.TaskCat.Name,
                Header = taskEntry.Header,
                Content = taskEntry.Content,
                Hide = taskEntry.Hide,
                Solved = taskEntry.Solved,
                Price = taskEntry.Price,
                Subcategory = taskEntry.TaskSubcategoryId,
                SubcategoryName = taskEntry.TaskSubcategory.Name,
                Creator = taskEntry.Creator.MapToUserShortModel(),
                Executive = taskEntry.Executive?.MapToUserShortModel(),
                Likes = taskEntry.TaskLikes.Select(l1 => l1.Liker).Select(u => u.MapToUserShortModel()).ToList(),
                Subscribers = taskEntry.TaskSubscribers.Select(ts => new TaskSubscriberModel()
                {
                    Id = ts.TaskSubscriberId,
                    Price = ts.Price,
                    Subscriber = ts.Subscriber.MapToUserShortModel()
                }).ToList(),
                Comments = taskEntry.TaskComments.Select(tc => new TaskCommentModel()
                {
                    Id = tc.TaskCommentId,
                    Content = tc.Content,
                    TaskId = tc.TaskId,
                    Creator = tc.Creator.MapToUserShortModel()
                }).ToList(),
                Attachments = taskEntry.Attachments.Select(f => new FileModel()
                {
                    Id = f.File.FileId,
                    RealFileName = f.File.RealFileName,
                    FileName = f.File.FileName
                }).ToList()
            };
        }
    }
}
