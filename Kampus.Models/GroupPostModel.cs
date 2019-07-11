using System;
using System.Collections.Generic;

namespace Kampus.Models
{
    public class GroupPostModel: Entity
    {
        public string Content { get; set; }
        public int GroupId { get; set; }

        public UserShortModel Creator { get; set; }

        public DateTime CreationTime { get; set; }

        public virtual List<UserShortModel> Likes { get; set; }
        public virtual List<GroupPostCommentModel> Comments { get; set; }
    }
}