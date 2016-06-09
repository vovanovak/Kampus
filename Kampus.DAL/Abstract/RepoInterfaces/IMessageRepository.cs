using System;
using System.Collections.Generic;
using System.Linq;
using Kampus.Entities;
using Kampus.Models;

namespace Kampus.DAL.Abstract
{
    public interface IMessageRepository: IRepository<MessageModel>
    {
        List<MessageModel> GetUserMessages(int userid);
        void WriteMessage(int senderid, int receiverid, string text, List<FileModel> attachments);
        List<MessageModel> GetNewMessages(int senderid, int receiverid, int lastmsgid);
        List<MessageModel> GetMessages(int senderid, int receiverid);
        List<UserShortModel> GetUserMessangers(int userid);
    }
}