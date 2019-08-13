using Kampus.Persistence.Entities.MessageRelated;

namespace Kampus.Persistence.Entities.AttachmentsRelated
{
    public class MessageFile
    {
        public int MessageId { get; set; }
        public Message Message { get; set; }

        public int FileId { get; set; }
        public File File { get; set; }
    }
}
