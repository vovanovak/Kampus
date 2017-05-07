using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kampus.Entities
{
    public class Group : DbEntity
    {
       
        public string Name { get; set; }
        public string Status { get; set; }
        public string Avatar { get; set; }

        public int? CreatorId { get; set; }
        public virtual User Creator { get; set; }

        public virtual List<User> Admins { get; set; }

        public virtual List<User> Members { get; set; }
        public virtual List<GroupPost> Posts { get; set; } 
    }
}
