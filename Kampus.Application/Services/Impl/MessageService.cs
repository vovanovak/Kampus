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
using System.Threading.Tasks;
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
            return _context.Messages
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .Include(m => m.Attachments)
                .ThenInclude(mf => mf.File);
        }

        public async Task<IReadOnlyList<MessageModel>> GetUserMessages(int userId)
        {
            return await GetMessages()
                .Where(m => m.SenderId == userId || m.ReceiverId == userId)
                .Select(m => _messageMapper.Map(m))
                .ToListAsync();
        }

        public async Task WriteMessage(int senderId, int receiverId, string text, List<FileModel> attachments)
        {
            Message msg = new Message();

            var sender = await _context.Users.SingleAsync(u => u.UserId == senderId);
            var receiver = await _context.Users.SingleAsync(u => u.UserId == receiverId);

            msg.Sender = sender;
            msg.SenderId = senderId;

            msg.Receiver = receiver;
            msg.ReceiverId = receiverId;

            msg.Content = text;
            msg.CreationDate = DateTime.Now;

            if (attachments != null)
            {
                var files = new List<File>();
                foreach (var link in attachments)
                {
                    var file = new File {RealFileName = link.RealFileName, FileName = link.FileName};

                    files.Add(file);
                    _context.Files.Add(file);
                }

                msg.Attachments = files.Select(f => new MessageFile { File = f, Message = msg }).ToList();
            }

            var notification = new Notification(DateTime.Now, NotificationType.Message,
                sender, receiver, "/Home/Conversation?username=" + receiver.Username, " надіслав вам повідомлення");

            _context.Notifications.Add(notification);
            _context.Messages.Add(msg);

            await _context.SaveChangesAsync();
        }

        public async Task<IReadOnlyList<MessageModel>> GetNewMessages(int senderId, int receiverId, int lastMsgId)
        {
            var creationDate = (await _context.Messages.SingleAsync(m1 => m1.MessageId == lastMsgId)).CreationDate;

            return await GetMessages()
                .Where(m => (m.SenderId == senderId && m.ReceiverId == receiverId ||
                             m.SenderId == receiverId && m.ReceiverId == senderId) &&
                            m.CreationDate > creationDate)
                .Select(m => _messageMapper.Map(m))
                .ToListAsync();
            ;
        }

        public async Task<IReadOnlyList<MessageModel>> GetMessages(int senderId, int receiverId)
        {
            return await GetMessages()
                .Where(m => m.SenderId == senderId && m.ReceiverId == receiverId ||
                            m.ReceiverId == senderId && m.SenderId == receiverId)
                .Select(m => _messageMapper.Map(m))
                .ToListAsync();
        }

        public async Task<IReadOnlyList<UserShortModel>> GetUserMessangers(int userId)
        {
            var messages = await GetMessages().Where(m => m.SenderId == userId || m.ReceiverId == userId).ToListAsync();
            var messangers = new List<UserShortModel>();

            foreach (var message in messages)
            {
                if (messangers.Count(m => m.Id == message.SenderId) == 0)
                {
                    if (message.Sender == null)
                        message.Sender = await _context.Users.SingleAsync(u => message.SenderId == u.UserId);

                    messangers.Add(new UserShortModel(message.Sender.UserId, message.Sender.Username, message.Sender.Avatar));
                }

                if (messangers.Count(m => m.Id == message.ReceiverId) == 0)
                {
                    if (message.Receiver == null)
                        message.Receiver = await _context.Users.SingleAsync(u => message.ReceiverId == u.UserId);

                    messangers.Add(new UserShortModel(message.Receiver.UserId, message.Receiver.Username, message.Receiver.Avatar));
                }
            }

            messangers.RemoveAll(u => u.Id == userId);

            return messangers;
        }

        public async Task<IReadOnlyDictionary<UserShortModel, MessageModel>> GetNewUserMessangers(int senderId)
        {
            return await _context.Messages.Where(m => m.ReceiverId == senderId &&
                                                      !_context.Messages.Any(m1 => m1.ReceiverId == senderId))
                .Select(m => _messageMapper.Map(m))
                .ToDictionaryAsync(m => m.Sender, m => m);
        }
    }
}
