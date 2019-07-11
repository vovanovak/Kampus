using System.Collections.Generic;

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
}
