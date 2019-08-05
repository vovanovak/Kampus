using Kampus.Persistence.Entities.AttachmentsRelated;
using Kampus.Persistence.Entities.UserRelated;
using System;
using System.Collections.Generic;

namespace Kampus.Persistence.Entities.MessageRelated
{
    public class Message
    {
        public int MessageId { get; set; }

        public int SenderId { get; set; }
        public User Sender { get; set; }

        public int ReceiverId { get; set; }
        public User Receiver { get; set; }

        public string Content { get; set; }

        public DateTime CreationDate { get; set; }

        public virtual List<File> Attachments { get; set; }
    }
}
