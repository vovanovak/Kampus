using Kampus.Persistence.Entities.WallPostRelated;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kampus.Persistence.Entities.AttachmentsRelated
{
    public class File
    {
        public int FileId { get; set; }
        public string FileName { get; set; }
        public string RealFileName { get; set; }
    }
}
