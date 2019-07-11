namespace Kampus.Models
{
    public class GroupPostCommentModel : Entity
    {
        public string Content { get; set; }

        public UserShortModel Creator { get; set; }

        public int GroupId { get; set; }
    }
}