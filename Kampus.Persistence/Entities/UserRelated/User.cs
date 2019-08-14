using Kampus.Persistence.Entities.MessageRelated;
using Kampus.Persistence.Entities.TaskRelated;
using Kampus.Persistence.Entities.WallPostRelated;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kampus.Persistence.Entities.UserRelated
{
    public class User
    {
        public int UserId { get; set; }

        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Status { get; set; }
        public string Avatar { get; set; }
        public string Fullname { get; set; }
        public double Rating { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime DateOfBirth { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime NotificationsLastChecked { get; set; }

        public int StudentDetailsId { get; set; }
        public StudentDetails StudentDetails { get; set; }

        public int RoleId { get; set; }
        public Role Role { get; set; }

        public int CityId { get; set; }
        public City City { get; set; }

        public int UserPermissionsId { get; set; }
        public UserPermissions UserPermissions { get; set; }

        public List<TaskEntry> Tasks { get; set; }
        public List<Message> Messages { get; set; }
        public List<WallPost> Posts { get; set; }
        public List<Friend> Friends { get; set; }
        public List<Subscriber> Subscribers { get; set; }
        public List<BlackList> BlackList { get; set; }
        public List<Achievement> Achievements { get; set; }
    }
}
