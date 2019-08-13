using Kampus.Persistence.Entities.WallPostRelated;

namespace Kampus.Persistence.Entities.AttachmentsRelated
{
    public class WallPostFile
    {
        public int WallPostId { get; set; }
        public WallPost WallPost { get; set; }

        public int FileId { get; set; }
        public File File { get; set; }
    }
}
