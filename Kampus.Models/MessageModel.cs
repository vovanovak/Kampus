using System;
using System.Collections.Generic;

namespace Kampus.Models
{
    public class MessageModel: Entity
    {
        public UserShortModel Sender { get; set; }
        public UserShortModel Receiver { get; set; }
        public DateTime CreationDate { get; set; }
        public string Content { get; set; }
        public List<FileModel> Attachments { get; set; }
    }
}