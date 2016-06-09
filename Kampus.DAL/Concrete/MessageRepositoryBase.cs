using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Kampus.DAL.Abstract;
using Kampus.Entities;
using Kampus.Models;

namespace Kampus.DAL.Concrete
{
    public class MessageRepositoryBase : RepositoryBase<MessageModel, Message>, IMessageRepository
    {
        private List<string> _imageExtensions = new List<string>() { ".jpg" };

        protected override DbSet<Message> GetTable()
        {
            return ctx.Messages;
        }

        protected override Expression<Func<Message, MessageModel>> GetConverter()
        {
            return m => new MessageModel()
            {
                Id = m.Id,
                Content = m.Content,
                CreationDate = m.CreationDate,
                Receiver =
                    new UserShortModel()
                    {
                        Id = m.Receiver.Id,
                        Username = m.Receiver.Username,
                        Avatar = m.Receiver.Avatar
                    },
                Sender =
                    new UserShortModel()
                    {
                        Id = m.Sender.Id,
                        Username = m.Sender.Username,
                        Avatar = m.Sender.Avatar
                    },
                Attachments = m.Attachments.Select(f => new FileModel() { Id = f.Id, 
                    RealFileName = f.RealFileName,
                    FileName = f.FileName, 
                    Extension = f.FileName.Substring(f.FileName.IndexOf(".")),
                    IsImage = f.FileName.Substring(f.FileName.IndexOf(".")) == ".jpg"}).ToList()
            };
        }

        protected override void UpdateEntry(Message dbEntity, MessageModel entity)
        {
            dbEntity.Id = entity.Id;
            dbEntity.Content = entity.Content;
            dbEntity.CreationDate = entity.CreationDate;
            dbEntity.Receiver = ctx.Users.First(u => u.Id == entity.Receiver.Id);
            dbEntity.Sender = ctx.Users.First(u => u.Id == entity.Sender.Id);
        }

        public List<MessageModel> GetUserMessages(int userid)
        {
            return
                ctx.Messages.Where(m => m.SenderId == userid || 
                    m.ReceiverId == userid).Select(GetConverter()).ToList();
        }

        public void WriteMessage(int senderid, int receiverid, string text, List<FileModel> attachments)
        {
            Message msg = new Message();

            User sender = ctx.Users.First(u => u.Id == senderid);
            User receiver = ctx.Users.First(u => u.Id == receiverid);

            msg.Sender = sender;
            msg.SenderId = senderid;

            msg.Receiver = receiver;
            msg.ReceiverId = receiverid;

            msg.Content = text;
            msg.CreationDate = DateTime.Now;

           

            if (attachments != null)
            {
                List<File> files = new List<File>();
                foreach (var link in attachments)
                {
                    File file = new File();
                    file.RealFileName = link.RealFileName;
                    file.FileName = link.FileName;

                    files.Add(file);
                    ctx.Files.Add(file);
                }
                msg.Attachments = files;
            }
           

            Notification notification = new Notification();
            notification.Date = DateTime.Now;
            notification.Type = NotificationType.Message;
            notification.Sender = sender;
            notification.SenderId = senderid;
            notification.UserId = receiver.Id;
            notification.User = receiver;
            notification.Link = "/Home/Conversation?username=" + receiver.Username;
            notification.Message = " надіслав вам повідомлення";

            ctx.Notifications.Add(notification);

            ctx.Messages.Add(msg);
            ctx.SaveChanges();
        }

        public List<MessageModel> GetNewMessages(int senderid, int receiverid, int lastmsgid)
        {
            DateTime time = ctx.Messages.FirstOrDefault(m1 => m1.Id == lastmsgid).CreationDate;

            List<MessageModel> messages = ctx.Messages.Where(m =>
                ((m.SenderId == senderid && m.ReceiverId == receiverid) ||
                (m.SenderId == receiverid && m.ReceiverId == senderid)) &&
                m.CreationDate > time).Select(GetConverter()).ToList();

            return messages;
        }

        public List<MessageModel> GetMessages(int senderid, int receiverid)
        {
            List<MessageModel> messages =
                ctx.Messages.Where(m => (m.SenderId == senderid &&
                    m.ReceiverId == receiverid) ||
                    (m.ReceiverId == senderid &&
                    m.SenderId == receiverid))
                    .Select(GetConverter())
                    .ToList();

            return messages;
        }

        public List<UserShortModel> GetUserMessangers(int userid)
        {
            List<Message> messages = ctx.Messages.Where(m => m.SenderId == userid || m.ReceiverId == userid).ToList();
            List<UserShortModel> messangers = new List<UserShortModel>();

            foreach (Message message in messages)
            {
                if (messangers.Count(m => m.Id == message.SenderId) == 0)
                {
                    if (message.Sender == null)
                        message.Sender = ctx.Users.First(u => message.SenderId == u.Id);

                    messangers.Add(new UserShortModel()
                    {
                        Id = message.Sender.Id,
                        Username = message.Sender.Username,
                        Avatar = message.Sender.Avatar
                    });
                }

                if (messangers.Count(m => m.Id == message.ReceiverId) == 0)
                {
                    if (message.Receiver == null)
                        message.Receiver = ctx.Users.First(u => message.ReceiverId == u.Id);

                    messangers.Add(new UserShortModel()
                    {
                        Id = message.Receiver.Id,
                        Username = message.Receiver.Username,
                        Avatar = message.Receiver.Avatar
                    });
                }
            }

            return messangers;
        }
    }
}
