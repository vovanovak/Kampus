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

        public decimal? Price { get; set; }

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
}
