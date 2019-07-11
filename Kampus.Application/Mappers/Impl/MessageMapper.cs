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
                Id = message.Id,
                Content = message.Content,
                CreationDate = message.CreationDate,
                Receiver = new UserShortModel() { Id = message.Receiver.Id, Username = message.Receiver.Username, Avatar = message.Receiver.Avatar },
                Sender = new UserShortModel() { Id = message.Sender.Id, Username = message.Sender.Username, Avatar = message.Sender.Avatar },
                Attachments = message.Attachments.Select(f => new FileModel()
                {
                    Id = f.Id,
                    RealFileName = f.RealFileName,
                    FileName = f.FileName
                }).ToList()
            };
        }
    }
}
