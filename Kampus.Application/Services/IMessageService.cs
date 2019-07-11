using Kampus.Models;
using System.Collections.Generic;

namespace Kampus.Application.Services
{
    public interface IMessageService
    {
        IReadOnlyList<MessageModel> GetUserMessages(int userId);
        void WriteMessage(int senderId, int receiverId, string text, List<FileModel> attachments);
        IReadOnlyList<MessageModel> GetNewMessages(int senderId, int receiverId, int lastmsgid);
        IReadOnlyList<MessageModel> GetMessages(int senderId, int receiverId);
        IReadOnlyList<UserShortModel> GetUserMessangers(int userId);
        IReadOnlyDictionary<UserShortModel, MessageModel> GetNewUserMessangers(int senderId);
    }
}
