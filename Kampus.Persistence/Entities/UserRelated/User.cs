using Kampus.Persistence.Entities.GroupRelated;
using Kampus.Persistence.Entities.MessageRelated;
using Kampus.Persistence.Entities.TaskRelated;
using Kampus.Persistence.Entities.WallPostRelated;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kampus.Persistence.Entities.UserRelated
{
    public class User : DbEntity
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Status { get; set; }
        public string Avatar { get; set; }
        public string Fullname { get; set; }
        public float Rating { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime DateOfBirth { get; set; }
        public DateTime NotificationsLastChecked { get; set; }

        public int? StudentDetailsId { get; set; }
        public virtual StudentDetails StudentDetails { get; set; }

        public int? RoleId { get; set; }
        public virtual UserRole Role { get; set; }

        public int? CityId { get; set; }
        public virtual City City { get; set; }

        public int? PermissionsId { get; set; }
        public virtual UserPermissions Permissions { get; set; }

        public virtual List<Task> Tasks { get; set; }
        public virtual List<Message> Messages { get; set; }
        public virtual List<WallPost> Posts { get; set; }
        public virtual List<Group> Groups { get; set; }
        public virtual List<User> Friends { get; set; }
        public virtual List<User> Subscribers { get; set; }
        public virtual List<User> BlackList { get; set; }
        public virtual List<TaskCategory> Achievements { get; set; }
    }
}
