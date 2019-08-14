using System;
using System.Collections.Generic;
using System.Text;
using Kampus.Persistence.Entities.UserRelated;

namespace Kampus.Persistence.Entities.TaskRelated
{
    public class Achievement
    {
        public int AchievementId { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public int TaskCategoryId { get; set; }
        public TaskCategory TaskCategory { get; set; }
    }
}
