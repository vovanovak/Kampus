using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Kampus.Models
{
    public class GroupModel: Entity
    {
        [Required(ErrorMessage = "Please provide the group name", AllowEmptyStrings = false)]
        public string Name { get; set; }

        public string Status { get; set; }

        [DataType(DataType.ImageUrl)]
        public string Avatar { get; set; }

        public UserShortModel Creator { get; set; }

        public virtual List<UserShortModel> Admins { get; set; }
        public virtual List<UserShortModel> Members { get; set; }
        public virtual List<GroupPostModel> Posts { get; set; }
    }
}