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
using Microsoft.EntityFrameworkCore;

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

        private IQueryable<Message> GetMessages()
        {
            return _context.Messages.Include(m => m.Attachments).ThenInclude(mf => mf.File);
        }

        public List<MessageModel> GetUserMessages(int userId)
        {
            return GetMessages().Where(m => m.SenderId == userId || m.ReceiverId == userId)
                .Select(_messageMapper.Map)
                .ToList();
        }

        public void WriteMessage(int senderId, int receiverId, string text, List<FileModel> attachments)
        {
            Message msg = new Message();

            User sender = _context.Users.First(u => u.UserId == senderId);
            User receiver = _context.Users.First(u => u.UserId == receiverId);

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

                msg.Attachments = files.Select(f => new MessageFile { File = f, Message = msg }).ToList();
            }

            var notification = new Notification(DateTime.Now, NotificationType.Message,
                sender, receiver, "/Home/Conversation?username=" + receiver.Username, " надіслав вам повідомлення");

            _context.Notifications.Add(notification);
            _context.Messages.Add(msg);

            _context.SaveChanges();
        }

        public List<MessageModel> GetNewMessages(int senderId, int receiverId, int lastmsgid)
        {
            var time = _context.Messages.First(m1 => m1.MessageId == lastmsgid).CreationDate;

            var messages = GetMessages().Where(m => (m.SenderId == senderId && m.ReceiverId == receiverId ||
                                                     m.SenderId == receiverId && m.ReceiverId == senderId) &&
                                                    m.CreationDate > time)
                .Select(_messageMapper.Map)
                .ToList();

            return messages;
        }

        public List<MessageModel> GetMessages(int senderId, int receiverId)
        {
            var messages = GetMessages().Where(m => m.SenderId == senderId && m.ReceiverId == receiverId ||
                                                    m.ReceiverId == senderId && m.SenderId == receiverId)
                .Select(_messageMapper.Map)
                .ToList();

            return messages;
        }

        public List<UserShortModel> GetUserMessangers(int userId)
        {
            var messages = _context.Messages.Where(m => m.SenderId == userId || m.ReceiverId == userId).ToList();
            var messangers = new List<UserShortModel>();

            foreach (var message in messages)
            {
                if (messangers.Count(m => m.Id == message.SenderId) == 0)
                {
                    if (message.Sender == null)
                        message.Sender = _context.Users.First(u => message.SenderId == u.UserId);

                    messangers.Add(new UserShortModel(message.Sender.UserId, message.Sender.Username, message.Sender.Avatar));
                }

                if (messangers.Count(m => m.Id == message.ReceiverId) == 0)
                {
                    if (message.Receiver == null)
                        message.Receiver = _context.Users.First(u => message.ReceiverId == u.UserId);

                    messangers.Add(new UserShortModel(message.Receiver.UserId, message.Receiver.Username, message.Receiver.Avatar));
                }
            }

            return messangers;
        }

        public Dictionary<UserShortModel, MessageModel> GetNewUserMessangers(int senderId)
        {
            return _context.Messages.Where(m => m.ReceiverId == senderId &&
                                                !_context.Messages.Any(m1 => m1.ReceiverId == senderId))
                .Select(_messageMapper.Map)
                .ToDictionary(m => m.Sender, m => m);
        }
    }
}
