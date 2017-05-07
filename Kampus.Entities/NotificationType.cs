using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kampus.Entities
{
    public enum NotificationType
    {
        Message, 
        Subscribed, 
        Friendship,
        WallPostComment, 
        WallPostLike, 
        WallPostWritten,
        GroupPostLike, 
        TaskComment,
        TaskLike,
        TaskSubscribed,
        CheckedAsTaskExecutive,
        Achievement,
        Solved
    }
}
