using Kampus.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kampus.Application.Services
{
    public interface IMessageService
    {
        Task<IReadOnlyList<MessageModel>> GetUserMessages(int userId);
        Task WriteMessage(int senderId, int receiverId, string text, List<FileModel> attachments);
        Task<IReadOnlyList<MessageModel>> GetNewMessages(int senderId, int receiverId, int lastMsgId);
        Task<IReadOnlyList<MessageModel>> GetMessages(int senderId, int receiverId);
        Task<IReadOnlyList<UserShortModel>> GetUserMessangers(int userId);
        Task<IReadOnlyDictionary<UserShortModel, MessageModel>> GetNewUserMessangers(int senderId);
    }
}
