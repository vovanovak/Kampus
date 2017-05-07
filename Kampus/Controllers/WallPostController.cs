using Kampus.DAL;
using Kampus.DAL.Abstract;
using Kampus.DAL.Enums;
using Kampus.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Kampus.Controllers
{
    public class WallPostController : Controller
    {
        //
        // GET: /WallPost/
        private IUnitOfWork _unitOfWork;
        private static List<FileModel> _attachmentsWallpost;

        public WallPostController()
        {
            _unitOfWork = UnitOfWorkResolver.UnitOfWork;
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

            _attachmentsWallpost.AddRange(FileController.UploadFilesToServer(HttpContext).ToArray());
        }

        [HttpPost]
        public string WriteWallPost(string text)
        {
            UserModel receiver = Session["UserProfile"] as UserModel;
            UserModel sender = Session["CurrentUser"] as UserModel;

            WallPostModel last = _unitOfWork.WallPosts.WriteWallPost(receiver.Id, sender.Id, text, _attachmentsWallpost);

            last.Id = _unitOfWork.WallPosts.GetAll().Last().Id;

            if (_attachmentsWallpost == null)
                _attachmentsWallpost = new List<FileModel>();
            _attachmentsWallpost.Clear();

            return JsonConvert.SerializeObject(new
            {
                Id = last.Id,
                Content = last.Content,
                Likes = last.Likes.Count,
                Sender = last.Sender.Username,
                Username = last.Owner.Username,
                Comments = last.Comments,
                Files = last.Attachments.Where(f => !f.IsImage()).ToList(),
                Images = last.Attachments.Where(f => f.IsImage()).ToList(),
                IsDeletable = (sender.Id == receiver.Id)
            });
        }
        
        [HttpPost]
        public LikeResult LikeWallPost(int postId)
        {
            var sender = Session["CurrentUser"] as UserModel;
            return _unitOfWork.WallPosts.LikeWallPost(sender.Id, postId);
        }

        [HttpPost]
        public string WritePostComment(string text, int postId)
        {
            var sender = Session["CurrentUser"] as UserModel;
            var comment = _unitOfWork.WallPosts.WritePostComment(sender.Id, postId, text);
            return JsonConvert.SerializeObject(comment);
        }

        [HttpPost]
        public string DeleteWallPost(int postId)
        {
            return JsonConvert.SerializeObject(new { Result = _unitOfWork.WallPosts.Delete(postId) });
        }

        public string GetNewWallPosts(int lastPostId)
        {
            var receiver = Session["UserProfile"] as UserModel;
            var sender = Session["CurrentUser"] as UserModel;

            return JsonConvert.SerializeObject(_unitOfWork.WallPosts
                .GetLastWallPosts(receiver.Id, sender.Id, lastPostId).Select(last => (new
                {
                    Id = last.Id,
                    Content = last.Content,
                    Likes = last.Likes.Count,
                    Sender = last.Sender.Username,
                    Username = last.Owner.Username,
                    Comments = last.Comments,
                    Files = last.Attachments.Where(f => !f.IsImage()).ToList(),
                    Images = last.Attachments.Where(f => f.IsImage()).ToList(),
                    IsDeletable = (last.Owner.Id == sender.Id)
                })).ToList());
        }

        [HttpGet]
        public string GetNewWallPostComments(int postId, int? postCommentId)
        {
            WallPostCommentModel[] comments = _unitOfWork.WallPosts.GetNewWallPostComments(postId, postCommentId).ToArray();
            return JsonConvert.SerializeObject(comments);
        }
    }
}
