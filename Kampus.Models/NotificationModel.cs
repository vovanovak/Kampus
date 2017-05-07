using System;
using Kampus.Entities;

namespace Kampus.Models
{
    public class NotificationModel: Entity
    {
        public string Message { get; set; }
        public string Link { get; set; }
        public DateTime Date { get; set; }
        public DateTime? SeenDate { get; set; }
        public bool Seen { get; set; }
        public UserShortModel Receiver { get; set; }
        public UserShortModel Sender { get; set; }
        public NotificationType Type { get; set; }
    }
}