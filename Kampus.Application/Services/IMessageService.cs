using Kampus.Models;
using System.Collections.Generic;

namespace Kampus.Application.Services
{
    public interface IMessageService
    {
        List<MessageModel> GetUserMessages(int userId);
        void WriteMessage(int senderId, int receiverId, string text, List<FileModel> attachments);
        List<MessageModel> GetNewMessages(int senderId, int receiverId, int lastmsgid);
        List<MessageModel> GetMessages(int senderId, int receiverId);
        List<UserShortModel> GetUserMessangers(int userId);
        Dictionary<UserShortModel, MessageModel> GetNewUserMessangers(int senderId);
    }
}
