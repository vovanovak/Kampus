using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kampus.Entities
{
    public class Notification: DbEntity
    {
        public string Message { get; set; }
        public string Link { get; set; }

        public DateTime Date { get; set; }
        public bool Seen { get; set; }
        public DateTime? SeenDate { get; set; }

        public int? UserId { get; set; }
        public User User { get; set; }
        public int? SenderId { get; set; }
        public User Sender { get; set; }

        public NotificationType Type { get; set; }
    }
}
