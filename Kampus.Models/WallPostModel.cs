using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kampus.Models
{
    public class WallPostModel: Entity
    {
        public string Content { get; set; }

        public UserShortModel Owner { get; set; }
        public UserShortModel Sender { get; set; }

        public List<UserShortModel> Likes { get; set; }
        public List<WallPostCommentModel> Comments { get; set; }
        public List<FileModel> Attachments { get; set; }
        public List<FileModel> Images { get; set; }
    }

    public class WallPostCommentModel: Entity
    {
        public DateTime CreationTime { get; set; }

        public string Content { get; set; }

        public UserShortModel Creator { get; set; }

        public int WallPostId { get; set; }
    }
}
