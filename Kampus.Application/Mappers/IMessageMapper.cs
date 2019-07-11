using Kampus.Models;
using Kampus.Persistence.Entities.MessageRelated;

namespace Kampus.Application.Mappers
{
    public interface IMessageMapper
    {
        MessageModel Map(Message message);
    }
}
