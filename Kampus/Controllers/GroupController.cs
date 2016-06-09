using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Autofac;
using Kampus.DAL;
using Kampus.DAL.Concrete;
using Kampus.Entities;
using Kampus.Models;

namespace Kampus.Controllers
{
    public class GroupController : Controller
    {
        //
        // GET: /Group/

        private readonly UserRepositoryBase _dbUser = Kampus.Container.Autofac.Container.Resolve<UserRepositoryBase>();
        private readonly GroupRepositoryBase _dbGroup = Kampus.Container.Autofac.Container.Resolve<GroupRepositoryBase>();

        public void InitViewBag(int userid, int groupid)
        {
            UserModel user = (UserModel)_dbUser.GetEntityById(userid);

            GroupModel group = (GroupModel)_dbGroup.GetEntityById(groupid);
            group.Posts = _dbGroup.GetGroupPosts(groupid);

            ViewBag.CurrentUser = user;
            ViewBag.Group = group;

            ViewBag.IsMember = group.Members.Any(u => u.Id == userid);

            ViewBag.IsAdmin = group.Admins.Any(u => u.Id == userid);
        }

        public ActionResult Id(int id)
        {
            Session.Add("CurrentGroupId", id);

            int userid = Convert.ToInt32(Session["CurrentUserId"]);

            InitViewBag(userid, id);

            return View("Index");
        }

        [HttpPost]
        public ActionResult SubscribeForTheGroup(int res)
        {
            int userid = Convert.ToInt32(Session["CurrentUserId"]);
            int groupid = Convert.ToInt32(Session["CurrentGroupId"]);

            _dbGroup.SubscribeForTheGroup(userid, groupid, res);

            InitViewBag(userid, groupid); 

            return View("Index");
        }

        [HttpPost]
        public ActionResult WriteGroupPost(string content)
        {
            int userid = Convert.ToInt32(Session["CurrentUserId"]);
            int groupid = Convert.ToInt32(Session["CurrentGroupId"]);

            _dbGroup.WriteGroupPost(userid, groupid, content);
           
            InitViewBag(userid, groupid);

            return View("Index");
        }

        [HttpPost]
        public ActionResult WriteGroupComment(string content, int postid)
        {
            int userid = Convert.ToInt32(Session["CurrentUserId"]);
            int groupid = Convert.ToInt32(Session["CurrentGroupId"]);

            _dbGroup.WriteGroupPostComment(userid, postid, content);

            InitViewBag(userid, groupid);

            return View("Index");
        }

        [HttpPost]
        public ActionResult LikeGroupPost(int postid)
        {
            int userid = Convert.ToInt32(Session["CurrentUserId"]);
            int groupid = Convert.ToInt32(Session["CurrentGroupId"]);

            _dbGroup.LikeGroupPost(userid, postid);

            InitViewBag(userid, groupid);

            return View("Index");
        }
    }
}
