using System.Collections.Generic;
using System.Linq;
using Kampus.Application.Services;
using Kampus.Host.Constants;
using Kampus.Host.Extensions;
using Kampus.Host.Services;
using Kampus.Models;
using Kampus.Persistence.Enums;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Kampus.Host.Controllers
{
    public class WallPostController : Controller
    {
        //
        // GET: /WallPost/
        private readonly IWallPostService _wallPostService;
        private readonly IFileService _fileService;

        private static List<FileModel> _attachmentsWallpost;

        public WallPostController(IWallPostService wallPostService, IFileService fileService)
        {
            _wallPostService = wallPostService;
            _fileService = fileService;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public void UploadFileWallpost()
        {
            if (_attachmentsWallpost == null)
                _attachmentsWallpost = new List<FileModel>();

            _attachmentsWallpost.AddRange(_fileService.UploadFilesToServer(HttpContext));
        }

        [HttpPost]
        public string WriteWallPost(string text)
        {
            UserModel receiver = HttpContext.Session.Get<UserModel>(SessionKeyConstants.UserProfile);
            UserModel sender = HttpContext.Session.Get<UserModel>(SessionKeyConstants.CurrentUser);

            WallPostModel last = _wallPostService.WriteWallPost(receiver.Id, sender.Id, text, _attachmentsWallpost);

            last.Id = _wallPostService.GetLastWallPostId();

            if (_attachmentsWallpost == null)
                _attachmentsWallpost = new List<FileModel>();
            _attachmentsWallpost.Clear();

            return JsonConvert.SerializeObject(new
            {
                last.Id,
                last.Content,
                Likes = last.Likes.Count,
                Sender = last.Sender.Username,
                last.Owner.Username,
                last.Comments,
                Files = last.Attachments.Where(f => !f.IsImage()).ToList(),
                Images = last.Attachments.Where(f => f.IsImage()).ToList(),
                IsDeletable = sender.Id == receiver.Id
            });
        }

        [HttpPost]
        public LikeResult LikeWallPost(int postId)
        {
            var sender = HttpContext.Session.Get<UserModel>(SessionKeyConstants.CurrentUser);
            return _wallPostService.LikeWallPost(sender.Id, postId);
        }

        [HttpPost]
        public string WritePostComment(string text, int postId)
        {
            var sender = HttpContext.Session.Get<UserModel>(SessionKeyConstants.CurrentUser);
            var comment = _wallPostService.WritePostComment(sender.Id, postId, text);
            return JsonConvert.SerializeObject(comment);
        }

        [HttpPost]
        public string DeleteWallPost(int postId)
        {
            _wallPostService.Delete(postId);
            return JsonConvert.SerializeObject(new { Result = true });
        }

        public string GetNewWallPosts(int lastPostId)
        {
            var receiver = HttpContext.Session.Get<UserModel>(SessionKeyConstants.UserProfile);
            var sender = HttpContext.Session.Get<UserModel>(SessionKeyConstants.CurrentUser);

            return JsonConvert.SerializeObject(_wallPostService
                .GetLastWallPosts(receiver.Id, sender.Id, lastPostId).Select(last => (new
                {
                    last.Id,
                    last.Content,
                    Likes = last.Likes.Count,
                    Sender = last.Sender.Username,
                    last.Owner.Username,
                    last.Comments,
                    Files = last.Attachments.Where(f => !f.IsImage()).ToList(),
                    Images = last.Attachments.Where(f => f.IsImage()).ToList(),
                    IsDeletable = last.Owner.Id == sender.Id
                })).ToList());
        }

        [HttpGet]
        public string GetNewWallPostComments(int postId, int? postCommentId)
        {
            var comments = _wallPostService.GetNewWallPostComments(postId, postCommentId);
            return JsonConvert.SerializeObject(comments);
        }
    }
}
