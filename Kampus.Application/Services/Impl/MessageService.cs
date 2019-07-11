using Kampus.Application.Mappers;
using Kampus.Models;
using Kampus.Persistence.Contexts;
using Kampus.Persistence.Entities.AttachmentsRelated;
using Kampus.Persistence.Entities.MessageRelated;
using Kampus.Persistence.Entities.NotificationRelated;
using Kampus.Persistence.Entities.UserRelated;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kampus.Application.Services.Impl
{
    internal class MessageService : IMessageService
    {
        private readonly KampusContext _context;
        private readonly IMessageMapper _messageMapper;

        public MessageService(KampusContext context, IMessageMapper messageMapper)
        {
            _context = context;
            _messageMapper = messageMapper;
        }

        public IReadOnlyList<MessageModel> GetUserMessages(int userId)
        {
            return
                _context.Messages.Where(m => m.SenderId == userId ||
                    m.ReceiverId == userId).Select(_messageMapper.Map).ToList();
        }

        public void WriteMessage(int senderId, int receiverId, string text, List<FileModel> attachments)
        {
            Message msg = new Message();

            User sender = _context.Users.First(u => u.Id == senderId);
            User receiver = _context.Users.First(u => u.Id == receiverId);

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
                    _context.Files.Add(file);
                }
                msg.Attachments = files;
            }

            Notification notification = Notification.From(DateTime.Now, NotificationType.Message,
                sender, receiver, "/Home/Conversation?username=" + receiver.Username, " надіслав вам повідомлення");

            _context.Notifications.Add(notification);
            _context.Messages.Add(msg);

            _context.SaveChanges();
        }

        public IReadOnlyList<MessageModel> GetNewMessages(int senderId, int receiverId, int lastmsgid)
        {
            DateTime time = _context.Messages.FirstOrDefault(m1 => m1.Id == lastmsgid).CreationDate;

            List<MessageModel> messages = _context.Messages.Where(m =>
                ((m.SenderId == senderId && m.ReceiverId == receiverId) ||
                (m.SenderId == receiverId && m.ReceiverId == senderId)) &&
                m.CreationDate > time).Select(_messageMapper.Map).ToList();

            return messages;
        }

        public IReadOnlyList<MessageModel> GetMessages(int senderId, int receiverId)
        {
            List<MessageModel> messages =
                _context.Messages.Where(m => (m.SenderId == senderId &&
                    m.ReceiverId == receiverId) ||
                    (m.ReceiverId == senderId &&
                    m.SenderId == receiverId))
                    .Select(_messageMapper.Map)
                    .ToList();

            return messages;
        }

        public IReadOnlyList<UserShortModel> GetUserMessangers(int userId)
        {
            List<Message> messages = _context.Messages.Where(m => m.SenderId == userId || m.ReceiverId == userId).ToList();
            List<UserShortModel> messangers = new List<UserShortModel>();

            foreach (Message message in messages)
            {
                if (messangers.Count(m => m.Id == message.SenderId) == 0)
                {
                    if (message.Sender == null)
                        message.Sender = _context.Users.First(u => message.SenderId == u.Id);

                    messangers.Add(UserShortModel.From(message.Sender.Id, message.Sender.Username, message.Sender.Avatar));
                }

                if (messangers.Count(m => m.Id == message.ReceiverId) == 0)
                {
                    if (message.Receiver == null)
                        message.Receiver = _context.Users.First(u => message.ReceiverId == u.Id);

                    messangers.Add(UserShortModel.From(message.Receiver.Id, message.Receiver.Username, message.Receiver.Avatar));
                }
            }

            return messangers;
        }

        public IReadOnlyDictionary<UserShortModel, MessageModel> GetNewUserMessangers(int senderId)
        {
            return _context.Messages.Where(m => m.ReceiverId == senderId &&
                    !_context.Messages.Any(m1 => m1.ReceiverId == senderId))
                    .Select(_messageMapper.Map)
                    .Select(m => new
                    {
                        Key = m.Sender,
                        Value = m
                    })
                    .ToDictionary(pair => pair.Key, pair => pair.Value);
        }
    }
}
