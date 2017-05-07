using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Kampus.DAL.Abstract;
using Kampus.Entities;
using Kampus.Models;
using Kampus.DAL.Abstract.Repositories;

namespace Kampus.DAL.Concrete.Repositories
{
    internal class MessageRepositoryBase : RepositoryBase<MessageModel, Message>, IMessageRepository
    {
        private List<string> _imageExtensions = new List<string>() { ".jpg", ".png" };

        public MessageRepositoryBase(KampusContext context) : base(context)
        {
        }

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
                Receiver = new UserShortModel() { Id = m.Receiver.Id, Username = m.Receiver.Username, Avatar = m.Receiver.Avatar },
                Sender = new UserShortModel() { Id = m.Sender.Id, Username = m.Sender.Username, Avatar = m.Sender.Avatar },
                Attachments = m.Attachments.Select(f => new FileModel()
                {
                    Id = f.Id,
                    RealFileName = f.RealFileName,
                    FileName = f.FileName
                }).ToList()
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

        public List<MessageModel> GetUserMessages(int userId)
        {
            return
                ctx.Messages.Where(m => m.SenderId == userId || 
                    m.ReceiverId == userId).Select(GetConverter()).ToList();
        }

        public void WriteMessage(int senderId, int receiverId, string text, List<FileModel> attachments)
        {
            Message msg = new Message();

            User sender = ctx.Users.First(u => u.Id == senderId);
            User receiver = ctx.Users.First(u => u.Id == receiverId);

            msg.Sender = sender;
            msg.SenderId = senderId;

            msg.Receiver = receiver;
            msg.ReceiverId = receiverId;

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
           
            Notification notification = Notification.From(DateTime.Now, NotificationType.Message, 
                sender, receiver, "/Home/Conversation?username=" + receiver.Username, " надіслав вам повідомлення");
            
            ctx.Notifications.Add(notification);
            ctx.Messages.Add(msg);

            ctx.SaveChanges();
        }

        public List<MessageModel> GetNewMessages(int senderId, int receiverId, int lastmsgid)
        {
            DateTime time = ctx.Messages.FirstOrDefault(m1 => m1.Id == lastmsgid).CreationDate;

            List<MessageModel> messages = ctx.Messages.Where(m =>
                ((m.SenderId == senderId && m.ReceiverId == receiverId) ||
                (m.SenderId == receiverId && m.ReceiverId == senderId)) &&
                m.CreationDate > time).Select(GetConverter()).ToList();

            return messages;
        }

        public List<MessageModel> GetMessages(int senderId, int receiverId)
        {
            List<MessageModel> messages =
                ctx.Messages.Where(m => (m.SenderId == senderId &&
                    m.ReceiverId == receiverId) ||
                    (m.ReceiverId == senderId &&
                    m.SenderId == receiverId))
                    .Select(GetConverter())
                    .ToList();

            return messages;
        }

        public List<UserShortModel> GetUserMessangers(int userId)
        {
            List<Message> messages = ctx.Messages.Where(m => m.SenderId == userId || m.ReceiverId == userId).ToList();
            List<UserShortModel> messangers = new List<UserShortModel>();

            foreach (Message message in messages)
            {
                if (messangers.Count(m => m.Id == message.SenderId) == 0)
                {
                    if (message.Sender == null)
                        message.Sender = ctx.Users.First(u => message.SenderId == u.Id);

                    messangers.Add(UserShortModel.From(message.Sender.Id, message.Sender.Username, message.Sender.Avatar));
                }

                if (messangers.Count(m => m.Id == message.ReceiverId) == 0)
                {
                    if (message.Receiver == null)
                        message.Receiver = ctx.Users.First(u => message.ReceiverId == u.Id);

                    messangers.Add(UserShortModel.From(message.Receiver.Id, message.Receiver.Username, message.Receiver.Avatar));
                }
            }

            return messangers;
        }

        public Dictionary<UserShortModel, MessageModel> GetNewUserMessangers(int senderId)
        {
            return ctx.Messages.Where(m => m.ReceiverId == senderId && 
                    !ctx.Messages.Any(m1 => m1.ReceiverId == senderId))
                    .Select(GetConverter())
                    .Select(m => new
                    {
                        Key = m.Sender,
                        Value = m
                    })
                    .ToDictionary(pair => pair.Key, pair => pair.Value);
        }
    }
}
