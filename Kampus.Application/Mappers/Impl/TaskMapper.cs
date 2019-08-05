using System;
using System.Linq;
using Kampus.Models;
using Kampus.Persistence.Entities.TaskRelated;

namespace Kampus.Application.Mappers.Impl
{
    internal class TaskMapper : ITaskMapper
    {
        public TaskModel Map(TaskEntry taskEntry)
        {
            return new TaskModel()
            {
                Id = taskEntry.Id,
                Category = taskEntry.TaskCat.Id,
                CategoryName = taskEntry.TaskCat.Name,
                Header = taskEntry.Header,
                Content = taskEntry.Content,
                Hide = taskEntry.Hide,
                Solved = taskEntry.Solved,
                Price = taskEntry.Price,
                Subcategory = taskEntry.TaskSubcategoryId,
                SubcategoryName = taskEntry.TaskSubcategory.Name,
                Creator = new UserShortModel() { Id = taskEntry.Creator.Id, Username = taskEntry.Creator.Username, Avatar = taskEntry.Creator.Avatar },
                Executive = (taskEntry.Executive != null) ? new UserShortModel() { Id = taskEntry.Executive.Id, Username = taskEntry.Executive.Username, Avatar = taskEntry.Executive.Avatar } : null,
                Likes = taskEntry.TaskLikes.Select(l1 => l1.Liker).
                    Select(p2 => new UserShortModel() { Id = p2.Id, Username = p2.Username, Avatar = p2.Avatar }).ToList(),
                Subscribers = taskEntry.TaskSubscribers.Select(p2 => new TaskSubscriberModel() { Id = p2.Id, Price = p2.Price, Subscriber = new UserShortModel() { Id = p2.Subscriber.Id, Username = p2.Subscriber.Username, Avatar = p2.Subscriber.Avatar } }).ToList(),
                Comments = taskEntry.TaskComments.Select(t1 => new TaskCommentModel() { Content = t1.Content, Id = t1.Id, TaskId = t1.TaskId, Creator = new UserShortModel() { Id = t1.Creator.Id, Username = t1.Creator.Username, Avatar = t1.Creator.Avatar } }).ToList(),
                Attachments = taskEntry.Attachments.Select(f => new FileModel()
                {
                    Id = f.Id,
                    RealFileName = f.RealFileName,
                    FileName = f.FileName
                }).ToList()
            };
        }
    }
}
