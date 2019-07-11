using Kampus.Api.Constants;
using Kampus.Api.Controllers;
using Kampus.Api.Extensions;
using Kampus.Api.Services;
using Kampus.Application.Services;
using Kampus.Application.Services.Users;
using Kampus.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Kampus.Controllers
{
    public class MessageController : Controller
    {
        //
        // GET: /Message/

        private readonly IMessageService _messageService;
        private readonly IUserService _userService;
        private readonly IFileService _fileService;

        private static List<FileModel> _attachmentsMessages;

        public MessageController(IMessageService messageService, IUserService userService)
        {
            _messageService = messageService;
            _userService = userService;
        }

        public ActionResult Index()
        {
            UserModel sender = HttpContext.Session.Get<UserModel>(SessionKeyConstants.CurrentUser);

            List<MessageModel> messages = _messageService.GetUserMessages(sender.Id);
            List<UserShortModel> messangers = _messageService.GetUserMessangers(sender.Id);

            if (messangers.Any(u => u.Id == sender.Id))
                messangers.RemoveAll(u => u.Id == sender.Id);

            var firstOrDefault = messangers.FirstOrDefault();
            if (firstOrDefault != null)
            {
                UserModel receiver = _userService.GetByUsername(firstOrDefault.Username);

                ViewBag.Messangers = messangers;
                ViewBag.SecondUser = receiver.Username;

                List<MessageModel> toViewBag =
                    messangers.Select(u => messages.OrderBy(m => m.CreationDate).Last()).ToList();

                ViewBag.FirstMessages = toViewBag;

                ViewBag.CurrentUser = sender;
                ViewBag.UserProfile = receiver;

                ViewBag.Messages = _messageService.GetMessages(sender.Id, receiver.Id).OrderBy(m => m.CreationDate.Ticks);
            }
            else
            {
                ViewBag.CurrentUser = sender;
                ViewBag.UserProfile = new UserShortModel() { Id = -1 };
            }

            return View("Conversation");
        }

        public ActionResult Conversation(string username)
        {
            UserModel sender = HttpContext.Session.Get<UserModel>(SessionKeyConstants.CurrentUser);
            UserModel receiver = _userService.GetByUsername(username);

            List<MessageModel> messages = _messageService.GetUserMessages(sender.Id);

            List<UserShortModel> messangers = _messageService.GetUserMessangers(sender.Id);

            messangers.RemoveAll(u => u.Id == sender.Id);

            if (messangers.All(u => u.Username != username))
            {
                messangers.Add(UserShortModel.From(receiver));
            }

            ViewBag.Messangers = messangers;
            ViewBag.SecondUser = username;

            List<MessageModel> toViewBag = messangers.Select(u => messages.OrderBy(m => m.CreationDate).LastOrDefault()).ToList();
            
            ViewBag.FirstMessages = toViewBag;

            ViewBag.CurrentUser = sender;
            ViewBag.UserProfile = receiver;

            ViewBag.Messages = _messageService.GetMessages(sender.Id, receiver.Id);

            return View("Conversation");
        }

        [HttpPost]
        public void UploadFileMessage()
        {
            if (_attachmentsMessages == null)
                _attachmentsMessages = new List<FileModel>();

            var files = _fileService.UploadFilesToServer(HttpContext);
            _attachmentsMessages.AddRange(files);
        }

        [HttpPost]
        public string WriteMessage(int receiverId, string text)
        {
            UserModel sender = HttpContext.Session.Get<UserModel>(SessionKeyConstants.CurrentUser);
            UserModel receiver = _userService.GetById(receiverId);

            _messageService.WriteMessage(sender.Id, receiverId, text, _attachmentsMessages);

            List<MessageModel> messages = _messageService.GetUserMessages(sender.Id);

            List<MessageModel> models = _messageService.GetMessages(sender.Id, receiverId);
            ViewBag.Messages = models;

            if (_attachmentsMessages != null)
                _attachmentsMessages.Clear();

            MessageModel last = models.OrderBy(m => m.CreationDate.Ticks).Last();

            return JsonConvert.SerializeObject(new
            {
                Id = last.Id,
                Username = last.Sender.Username,
                Receiver = last.Receiver.Username,
                Content = last.Content,
                Avatar = last.Sender.Avatar,
                Files = last.Attachments.Where(a => !a.IsImage()),
                Images = last.Attachments.Where(a => a.IsImage()),
                IsSender = true
            });
        }

        [HttpGet]
        public string GetNewMessages(int senderId, int receiverId, int lastMsgId)
        {
            var messages = _messageService.GetNewMessages(senderId, receiverId, lastMsgId);
            //var messangers = _messageService.GetNewUserMessangers(senderId);

            return JsonConvert.SerializeObject(new
            {
                Messages = messages.Where(m => m.Sender.Id != senderId).Select(m => new
                {
                    Id = m.Id,
                    Username = m.Sender.Username,
                    Content = m.Content,
                    Avatar = m.Sender.Avatar,
                    Files = m.Attachments.Where(a => !a.IsImage()),
                    Images = m.Attachments.Where(a => a.IsImage()),
                    IsSender = false
                }).ToArray(),
              //  Messangers = messangers
            });
        }
    }
}
