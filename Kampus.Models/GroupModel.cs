using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Kampus.Entities;

namespace Kampus.Models
{
    public class GroupModel: Entity
    {
        [Required(ErrorMessage = "Please provide the group name", AllowEmptyStrings = false)]
        public string Name { get; set; }

        public string Status { get; set; }

        [DataType(System.ComponentModel.DataAnnotations.DataType.ImageUrl)]
        public string Avatar { get; set; }

        public UserShortModel Creator { get; set; }

        public virtual List<UserShortModel> Admins { get; set; }
        public virtual List<UserShortModel> Members { get; set; }
        public virtual List<GroupPostModel> Posts { get; set; }
    }

    public class GroupPostModel: Entity
    {
        public string Content { get; set; }
        public int GroupId { get; set; }

        public UserShortModel Creator { get; set; }

        public DateTime CreationTime { get; set; }

        public virtual List<UserShortModel> Likes { get; set; }
        public virtual List<GroupPostCommentModel> Comments { get; set; }
    }

    public class GroupPostCommentModel : Entity
    {
        public string Content { get; set; }

        public UserShortModel Creator { get; set; }

        public int GroupId { get; set; }
    }
}