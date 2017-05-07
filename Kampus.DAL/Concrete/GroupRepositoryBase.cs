using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using Kampus.DAL.Abstract;
using Kampus.Entities;
using Kampus.Models;

namespace Kampus.DAL.Concrete
{
    public class GroupRepositoryBase: RepositoryBase<GroupModel, Group>
    {
        protected override DbSet<Group> GetTable()
        {
            return ctx.Groups;
        }

        protected override Expression<Func<Group, GroupModel>> GetConverter()
        {
            return g => new GroupModel()
            {
                Id = g.Id,
                Avatar = g.Avatar,
                Name = g.Name,
                Status = g.Status,
                Admins = g.Admins.Select(u => new UserShortModel(){ Id = u.Id, Avatar = u.Avatar, Username = u.Username}).ToList(),
                Members = g.Members.Select(u => new UserShortModel() { Id = u.Id, Avatar = u.Avatar, Username = u.Username }).ToList(),
                Creator = new UserShortModel() { Id = g.Creator.Id, Avatar = g.Creator.Avatar, Username = g.Creator.Username }
            };
        }

        protected override void UpdateEntry(Group dbEntity, GroupModel entity)
        {
            dbEntity.Name = entity.Name;
            dbEntity.Status = entity.Status;
            dbEntity.Avatar = entity.Avatar;
            dbEntity.Members = new List<User>();
            dbEntity.Creator = ctx.Users.FirstOrDefault(u => u.Id == entity.Creator.Id);
            dbEntity.CreatorId = entity.Creator.Id;

            dbEntity.Admins = new List<User> {dbEntity.Creator};

            ctx.Groups.Add(dbEntity);

            dbEntity.Creator.Groups.Add(dbEntity);
        }

        public List<GroupPostModel> GetGroupPosts(int groupid)
        {
            List<GroupPostModel> posts = ctx.GroupPosts.Where(p => p.GroupId == groupid)
                .Select(p => new GroupPostModel()
                {
                    Id = p.Id,
                    Content = p.Content,
                    CreationTime = p.CreationTime,
                    GroupId = p.GroupId,
                    Creator = new UserShortModel() { Id = p.UserId, Username = p.User.Username, Avatar = p.User.Avatar},
                    Likes = p.Likes.Select(l => l.User).Select(l => new UserShortModel() { Id = l.Id, Username = l.Username, Avatar = l.Avatar }).ToList(),
                    Comments = p.Comments.Select(c => new GroupPostCommentModel() 
                    { Id = c.Id,
                      Content = c.Content,
                      GroupId = c.GroupPost.GroupId,
                      Creator = new UserShortModel() 
                        { Id = c.User.Id, 
                          Username = c.User.Username, 
                          Avatar = c.User.Avatar 
                        }
                    }).ToList()
                }).ToList();
            return posts;
        }

        public List<GroupModel> GetUserGroups(int userid)
        {
            return ctx.Groups.Where(g => g.Members.Any(u => u.Id == userid) || g.Admins.Any(u => u.Id == userid)).Select(GetConverter()).ToList();
        }

        public void CreateGroup(GroupModel model)
        {
            Save(model);
        }

        public void SubscribeForTheGroup(int userid, int groupid, int res)
        {
            User user = ctx.Users.First(u => u.Id == userid);
            Group group = ctx.Groups.First(g => g.Id == groupid);

            if (res == 1)
            {
                group.Members.Add(user);
                user.Groups.Add(group);
            }
            else
            {
                group.Members.Remove(user);
                user.Groups.Remove(group);
            }

            ctx.SaveChanges();
        }

        public void WriteGroupPost(int userid, int groupid, string content)
        {
            User user = ctx.Users.First(u => u.Id == userid);
            Group group = ctx.Groups.First(g => g.Id == groupid);

            GroupPost post = new GroupPost
            {
                Content = content,
                User = user,
                UserId = userid,
                Group = group,
                GroupId = groupid,
                CreationTime = DateTime.Now,
                Comments = new List<GroupPostComment>(),
                Likes = new List<GroupPostLike>()
            };


            ctx.GroupPosts.Add(post);
            ctx.SaveChanges();
        }

        public void WriteGroupPostComment(int userid, int postid, string content)
        {
            User user = ctx.Users.First(u => u.Id == userid);
            GroupPost post = ctx.GroupPosts.First(p => p.Id == postid);

            GroupPostComment comment = new GroupPostComment();
            comment.Content = content;
            comment.GroupPost = post;
            comment.GroupPostId = postid;
            comment.User = user;
            comment.UserId = userid;

            ctx.GroupPostComments.Add(comment);
            ctx.SaveChanges();
        }

        public void LikeGroupPost(int userid, int postid)
        {
            if (ctx.GroupPostLikes.Any(l => l.UserId == userid && l.Post.Id == postid))
            {
                List<GroupPostLike> likes =
                    ctx.GroupPostLikes.Where(l => l.UserId == userid && l.GroupPostId == postid).ToList();
                ctx.GroupPostLikes.RemoveRange(likes);
            }
            else
            {
                GroupPostLike like = new GroupPostLike();

                like.Post = ctx.GroupPosts.First(p => p.Id == postid);
                like.GroupPostId = like.Post.Id;

                like.User = ctx.Users.First(u => u.Id == userid);
                like.UserId = like.User.Id;

                ctx.GroupPostLikes.Add(like);
            }

            ctx.SaveChanges();
        }
    }
}