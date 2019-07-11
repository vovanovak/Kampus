using Kampus.Persistence.Entities.UserRelated;
using System;

namespace Kampus.Persistence.Entities.NotificationRelated
{
    public class Notification : DbEntity
    {
        public string Message { get; set; }
        public string Link { get; set; }

        public DateTime Date { get; set; }
        public bool Seen { get; set; }
        public DateTime? SeenDate { get; set; }

        public int? ReceiverId { get; set; }
        public User Receiver { get; set; }
        public int? SenderId { get; set; }
        public User Sender { get; set; }

        public NotificationType Type { get; set; }

        public Notification()
        {

        }
        public static Notification From(DateTime date, NotificationType type,
            User sender, User receiver, string link, string message)
        {
            Notification notification = new Notification();
            notification.Date = date;
            notification.Type = type;
            notification.Sender = sender;
            notification.SenderId = sender.Id;
            notification.ReceiverId = receiver.Id;
            notification.Receiver = receiver;
            notification.Link = link;
            notification.Message = message;
            return notification;
        }

    }
}
