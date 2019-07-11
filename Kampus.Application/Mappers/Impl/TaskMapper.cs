using System;
using System.Linq;
using Kampus.Models;
using Kampus.Persistence.Entities.TaskRelated;

namespace Kampus.Application.Mappers.Impl
{
    internal class TaskMapper : ITaskMapper
    {
        public TaskModel Map(Task task)
        {
            return new TaskModel()
            {
                Id = task.Id,
                Category = task.TaskCat.Id,
                CategoryName = task.TaskCat.Name,
                Header = task.Header,
                Content = task.Content,
                Hide = task.Hide,
                Solved = task.Solved,
                Price = task.Price,
                Subcategory = task.TaskSubcatId,
                SubcategoryName = task.TaskSubcat.Name,
                Creator = new UserShortModel() { Id = task.Creator.Id, Username = task.Creator.Username, Avatar = task.Creator.Avatar },
                Executive = (task.Executive != null) ? new UserShortModel() { Id = task.Executive.Id, Username = task.Executive.Username, Avatar = task.Executive.Avatar } : null,
                Likes = task.TaskLikes.Select(l1 => l1.Liker).
                    Select(p2 => new UserShortModel() { Id = p2.Id, Username = p2.Username, Avatar = p2.Avatar }).ToList(),
                Subscribers = task.TaskSubscribers.Select(p2 => new TaskSubscriberModel() { Id = p2.Id, Price = p2.Price, Subscriber = new UserShortModel() { Id = p2.Subscriber.Id, Username = p2.Subscriber.Username, Avatar = p2.Subscriber.Avatar } }).ToList(),
                Comments = task.TaskComments.Select(t1 => new TaskCommentModel() { Content = t1.Content, Id = t1.Id, TaskId = t1.TaskId, Creator = new UserShortModel() { Id = t1.Creator.Id, Username = t1.Creator.Username, Avatar = t1.Creator.Avatar } }).ToList(),
                Attachments = task.Attachments.Select(f => new FileModel()
                {
                    Id = f.Id,
                    RealFileName = f.RealFileName,
                    FileName = f.FileName
                }).ToList()
            };
        }
    }
}
