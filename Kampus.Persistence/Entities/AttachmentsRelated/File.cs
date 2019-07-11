using Kampus.Persistence.Entities.WallPostRelated;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kampus.Persistence.Entities.AttachmentsRelated
{
    public class File : DbEntity
    {
        public string FileName { get; set; }
        public string RealFileName { get; set; }

        public int? WallPostId { get; set; }
        public WallPost WallPost { get; set; }
    }
}
