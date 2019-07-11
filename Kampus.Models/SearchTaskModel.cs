namespace Kampus.Models
{
    public class SearchTaskModel : Entity
    {
        public string Request { get; set; }

        public int? CategoryId { get; set; }
        public int? SubcategoryId { get; set; }

        public int? MinPrice { get; set; }
        public int? MaxPrice { get; set; }
    }
}
