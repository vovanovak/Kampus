using System;
using System.Collections.Generic;
using System.Linq;
using Kampus.Entities;
using Kampus.Models;

namespace Kampus.DAL.Abstract.Repositories
{
    public interface IMessageRepository: IRepository<MessageModel>
    {
        List<MessageModel> GetUserMessages(int userId);
        void WriteMessage(int senderId, int receiverId, string text, List<FileModel> attachments);
        List<MessageModel> GetNewMessages(int senderId, int receiverId, int lastmsgid);
        List<MessageModel> GetMessages(int senderId, int receiverId);
        List<UserShortModel> GetUserMessangers(int userId);
        Dictionary<UserShortModel, MessageModel> GetNewUserMessangers(int senderId);
    }
}