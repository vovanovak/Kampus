using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kampus.Models
{
    public class TaskModel : Entity
    {
        [Required(ErrorMessage = "Please provide header of task", AllowEmptyStrings = false)]
        public string Header { get; set; }

        [Required(ErrorMessage = "Please provide content of task", AllowEmptyStrings = false)]
        public string Content { get; set; }

        public int? Price { get; set; }

        public UserShortModel Creator { get; set; }
        public UserShortModel Executive { get; set; }

        public bool? Hide { get; set; }

        public bool? Solved { get; set; }

        public List<TaskSubscriberModel> Subscribers { get; set; }
        public List<UserShortModel> Likes { get; set; }
        public List<TaskCommentModel> Comments { get; set; }

        public int? Category { get; set; }
        public string CategoryName { get; set; }

        public int? Subcategory { get; set; }
        public string SubcategoryName { get; set; }
        public List<FileModel> Attachments { get; set; }
    }

    public class TaskCommentModel: Entity
    {
        public string Content { get; set; }

        public DateTime CreationTime { get; set; }

        public UserShortModel User { get; set; }

        public int? TaskId { get; set; }
    }

    public class TaskSubscriberModel : Entity
    {
        public int? Price { get; set; }

        public UserShortModel User { get; set; }
    }

    public class TaskCategoryModel : Entity
    {
        public string Name { get; set; }
    }

    public class TaskSubcatModel: Entity
    {
        public string Name { get; set; }
        public int? TaskCategoryId { get; set; }
    }

    public class ExecutionReviewModel : Entity
    {
        public UserShortModel Executor { get; set; }

        public int? TaskId { get; set; }
        public TaskModel Task { get; set; }

        public int? Rating { get; set; }
        public string Review { get; set; }
    }

    public class SearchTaskModel : Entity
    {
        public string Request { get; set; }

        public int? CategoryId { get; set; }
        public int? SubcategoryId { get; set; }

        public int? MinPrice { get; set; }
        public int? MaxPrice { get; set; }
    }
}
