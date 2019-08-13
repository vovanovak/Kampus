using System.Linq;
using Kampus.Models;
using Kampus.Persistence.Entities.MessageRelated;

namespace Kampus.Application.Mappers.Impl
{
    internal class MessageMapper : IMessageMapper
    {
        public MessageModel Map(Message message)
        {
            return new MessageModel()
            {
                Id = message.MessageId,
                Content = message.Content,
                CreationDate = message.CreationDate,
                Receiver = new UserShortModel(message.Receiver.UserId, message.Receiver.Username, message.Receiver.Avatar),
                Sender = new UserShortModel(message.Sender.UserId, message.Sender.Username, message.Sender.Avatar),
                Attachments = message.Attachments.Select(mf => new FileModel()
                {
                    Id = mf.File.FileId,
                    RealFileName = mf.File.RealFileName,
                    FileName = mf.File.FileName
                }).ToList()
            };
        }
    }
}
