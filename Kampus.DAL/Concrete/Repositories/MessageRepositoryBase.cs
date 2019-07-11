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

       
    }
}
