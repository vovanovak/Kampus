using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kampus.Entities
{
    public class Message : DbEntity
    {
        public int? SenderId { get; set; }
        public User Sender { get; set; }

        public int? ReceiverId { get; set; }
        public User Receiver { get; set; }

        public string Content { get; set; }
        

        public DateTime CreationDate { get; set; }

        public virtual List<File> Attachments { get; set; }
    }
}
