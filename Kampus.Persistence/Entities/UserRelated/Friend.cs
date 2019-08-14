using System;
using System.Collections.Generic;
using System.Text;

namespace Kampus.Persistence.Entities.UserRelated
{
    public class Friend
    {
        public int FriendId { get; set; }

        public int User1Id { get; set; }
        public User User1 { get; set; }

        public int User2Id { get; set; }
        public User User2 { get; set; }
    }
}
