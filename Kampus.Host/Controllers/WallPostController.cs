using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public async Task<IActionResult> Index()
        {
            return View();
        }

        [HttpPost]
        public async Task UploadFileWallpost()
        {
            if (_attachmentsWallpost == null)
                _attachmentsWallpost = new List<FileModel>();

            _attachmentsWallpost.AddRange(await _fileService.UploadFilesToServer(HttpContext));
        }

        [HttpPost]
        public async Task<IActionResult> WriteWallPost(string text)
        {
            UserModel receiver = HttpContext.Session.Get<UserModel>(SessionKeyConstants.UserProfile);
            UserModel sender = HttpContext.Session.Get<UserModel>(SessionKeyConstants.CurrentUser);

            var last = await _wallPostService.WriteWallPost(receiver.Id, sender.Id, text, _attachmentsWallpost);

            last.Id = await _wallPostService.GetLastWallPostId();

            if (_attachmentsWallpost == null)
                _attachmentsWallpost = new List<FileModel>();
            _attachmentsWallpost.Clear();

            return Json(new
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
        public async Task<LikeResult> LikeWallPost(int postId)
        {
            var sender = HttpContext.Session.Get<UserModel>(SessionKeyConstants.CurrentUser);
            return await _wallPostService.LikeWallPost(sender.Id, postId);
        }

        [HttpPost]
        public async Task<IActionResult> WritePostComment(string text, int postId)
        {
            var sender = HttpContext.Session.Get<UserModel>(SessionKeyConstants.CurrentUser);
            var comment = await _wallPostService.WritePostComment(sender.Id, postId, text);
            return Json(comment);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteWallPost(int postId)
        {
            await _wallPostService.Delete(postId);
            return Json(new { Result = true });
        }

        public async Task<IActionResult> GetNewWallPosts(int lastPostId)
        {
            var receiver = HttpContext.Session.Get<UserModel>(SessionKeyConstants.UserProfile);
            var sender = HttpContext.Session.Get<UserModel>(SessionKeyConstants.CurrentUser);

            var result = (await _wallPostService.GetLastWallPosts(receiver.Id, sender.Id, lastPostId))
                .Select(last =>
                    new
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
                    }).ToList();

            return Json(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetNewWallPostComments(int postId, int? postCommentId)
        {
            var comments = await _wallPostService.GetNewWallPostComments(postId, postCommentId);
            return Json(comments);
        }
    }
}
