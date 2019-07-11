namespace Kampus.Models
{
    public class ExecutionReviewModel : Entity
    {
        public UserShortModel Executor { get; set; }

        public int? TaskId { get; set; }
        public TaskModel Task { get; set; }

        public int? Rating { get; set; }
        public string Review { get; set; }
    }
}
