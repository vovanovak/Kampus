using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kampus.Application.Services;
using Kampus.Application.Services.Users;
using Kampus.Host.Constants;
using Kampus.Host.Extensions;
using Kampus.Host.Services;
using Kampus.Models;
using Microsoft.AspNetCore.Mvc;

namespace Kampus.Host.Controllers
{
    public class MessageController : Controller
    {
        //
        // GET: /Message/

        private readonly IMessageService _messageService;
        private readonly IUserService _userService;
        private readonly IFileService _fileService;

        private static List<FileModel> _attachmentsMessages;

        public MessageController(IMessageService messageService, IUserService userService, IFileService fileService)
        {
            _messageService = messageService;
            _userService = userService;
            _fileService = fileService;
        }

        public async Task<IActionResult> Index()
        {
            var sender = HttpContext.Session.Get<UserModel>(SessionKeyConstants.CurrentUser);

            var messages = await _messageService.GetUserMessages(sender.Id);
            var messangers = await _messageService.GetUserMessangers(sender.Id);

            var firstOrDefault = messangers.FirstOrDefault();
            if (firstOrDefault != null)
            {
                var receiver = await _userService.GetByUsername(firstOrDefault.Username);

                ViewBag.Messangers = messangers;
                ViewBag.SecondUser = receiver.Username;

                var toViewBag = messangers.Select(u => messages.OrderBy(m => m.CreationDate).Last()).ToList();

                ViewBag.FirstMessages = toViewBag;

                ViewBag.CurrentUser = sender;
                ViewBag.UserProfile = receiver;

                ViewBag.Messages = (await _messageService.GetMessages(sender.Id, receiver.Id)).OrderBy(m => m.CreationDate.Ticks);
            }
            else
            {
                ViewBag.CurrentUser = sender;
                ViewBag.UserProfile = new UserShortModel() { Id = -1 };
            }

            return View("Conversation");
        }

        public async Task<IActionResult> Conversation(string username)
        {
            var sender = HttpContext.Session.Get<UserModel>(SessionKeyConstants.CurrentUser);
            var receiver = await _userService.GetByUsername(username);

            var messages = await _messageService.GetUserMessages(sender.Id);
            var messangers = (await _messageService.GetUserMessangers(sender.Id)).ToList();

            if (messangers.All(u => u.Username != username))
            {
                messangers.Add(UserShortModel.From(receiver));
            }

            ViewBag.Messangers = messangers;
            ViewBag.SecondUser = username;

            var toViewBag = messangers.Select(u => messages.OrderBy(m => m.CreationDate).LastOrDefault()).ToList();

            ViewBag.FirstMessages = toViewBag;

            ViewBag.CurrentUser = sender;
            ViewBag.UserProfile = receiver;

            ViewBag.Messages = _messageService.GetMessages(sender.Id, receiver.Id);

            return View("Conversation");
        }

        [HttpPost]
        public async Task UploadFileMessage()
        {
            if (_attachmentsMessages == null)
                _attachmentsMessages = new List<FileModel>();

            var files = await _fileService.UploadFilesToServer(HttpContext);
            _attachmentsMessages.AddRange(files);
        }

        [HttpPost]
        public async Task<IActionResult> WriteMessage(int receiverId, string text)
        {
            var sender = HttpContext.Session.Get<UserModel>(SessionKeyConstants.CurrentUser);

            await _messageService.WriteMessage(sender.Id, receiverId, text, _attachmentsMessages);

            var models = await _messageService.GetMessages(sender.Id, receiverId);

            ViewBag.Messages = models;

            if (_attachmentsMessages != null)
                _attachmentsMessages.Clear();

            var last = models.OrderBy(m => m.CreationDate.Ticks).Last();

            return Json(new
            {
                last.Id,
                last.Sender.Username,
                Receiver = last.Receiver.Username,
                last.Content,
                last.Sender.Avatar,
                Files = last.Attachments.Where(a => !a.IsImage()),
                Images = last.Attachments.Where(a => a.IsImage()),
                IsSender = true
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetNewMessages(int senderId, int receiverId, int lastMsgId)
        {
            var messages = await _messageService.GetNewMessages(senderId, receiverId, lastMsgId);

            return Json(new
            {
                Messages = messages.Where(m => m.Sender.Id != senderId).Select(m => new
                {
                    m.Id,
                    m.Sender.Username,
                    m.Content,
                    m.Sender.Avatar,
                    Files = m.Attachments.Where(a => !a.IsImage()),
                    Images = m.Attachments.Where(a => a.IsImage()),
                    IsSender = false
                }).ToArray()
            });
        }
    }
}
