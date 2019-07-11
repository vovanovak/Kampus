namespace Kampus.Models
{
    public class UserSearchModel
    {
        public string Request { get; set; }

        public string City { get; set; }

        public string University { get; set; }
        public string Faculty { get; set; }
        public int? Course { get; set; }

        public int? MinAge { get; set; }
        public int? MaxAge { get; set; }

        public int? MinRating { get; set; }
        public int? MaxRating { get; set; }
    }
}