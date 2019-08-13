using Kampus.Persistence.Entities.AttachmentsRelated;
using Kampus.Persistence.Entities.MessageRelated;
using Kampus.Persistence.Entities.NotificationRelated;
using Kampus.Persistence.Entities.TaskRelated;
using Kampus.Persistence.Entities.UniversityRelated;
using Kampus.Persistence.Entities.UserRelated;
using Kampus.Persistence.Entities.WallPostRelated;
using Microsoft.EntityFrameworkCore;

namespace Kampus.Persistence.Contexts
{
    public class KampusContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Friend> Friends { get; set; }
        public DbSet<Subscriber> Subscribers { get; set; }
        public DbSet<BlackList> BlackLists { get; set; }
        public DbSet<UserRecovery> Recoveries { get; set; }
        public DbSet<TaskExecutionReview> Reviews { get; set; }
        public DbSet<Role> UserRoles { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<StudentDetails> StudentDetails { get; set; }
        public DbSet<TaskEntry> Tasks { get; set; }
        public DbSet<TaskSubscriber> TaskSubscribers { get; set; }
        public DbSet<TaskComment> TaskComments { get; set; }
        public DbSet<TaskLike> TaskLikes { get; set; }
        public DbSet<TaskCategory> TaskCategories { get; set; }
        public DbSet<TaskSubcategory> TaskSubcategories { get; set; }
        public DbSet<University> Universities { get; set; }
        public DbSet<WallPost> WallPosts { get; set; }
        public DbSet<WallPostComment> WallPostComments { get; set; }
        public DbSet<WallPostLike> WallPostLikes { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Faculty> Faculties { get; set; }
        public DbSet<UserPermissions> Permissions { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<File> Files { get; set; }
        public DbSet<MessageFile> MessageFiles { get; set; }
        public DbSet<TaskFile> TaskFiles { get; set; }
        public DbSet<WallPostFile> WallPostFiles { get; set; }

        public KampusContext() : base()
        {
        }

        public KampusContext(DbContextOptions<KampusContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
