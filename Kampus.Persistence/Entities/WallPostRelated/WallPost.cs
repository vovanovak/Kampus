﻿using Kampus.Persistence.Entities.AttachmentsRelated;
using Kampus.Persistence.Entities.UserRelated;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Kampus.Persistence.Entities.WallPostRelated
{
    public class WallPost
    {
        public int WallPostId { get; set; }
        public string Content { get; set; }

        public int? OwnerId { get; set; }
        public virtual User Owner { get; set; }

        public int? SenderId { get; set; }
        public virtual User Sender { get; set; }

        public virtual List<WallPostComment> Comments { get; set; }
        public virtual List<WallPostLike> Likes { get; set; }
        public virtual List<WallPostFile> Attachments { get; set; }
    }
}
