using Kampus.Entities;
using Kampus.Persistence.Entities.AttachmentsRelated;
using Kampus.Persistence.Entities.GroupRelated;
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
        public DbSet<UserRecovery> Recoveries { get; set; }
        public DbSet<ExecutionReview> Reviews { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<StudentDetails> StudentDetails { get; set; }
        public DbSet<Task> Tasks { get; set; }
        public DbSet<TaskSubscriber> TaskSubscribers { get; set; }
        public DbSet<TaskComment> TaskComments { get; set; }
        public DbSet<TaskLike> TaskLikes { get; set; }
        public DbSet<TaskCategory> TaskCategories { get; set; }
        public DbSet<TaskSubcat> TaskSubcats { get; set; }
        public DbSet<University> Universities { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<GroupPost> GroupPosts { get; set; }
        public DbSet<GroupPostLike> GroupPostLikes { get; set; }
        public DbSet<GroupPostComment> GroupPostComments { get; set; }
        public DbSet<WallPost> UserPosts { get; set; }
        public DbSet<WallPostComment> UserPostComments { get; set; }
        public DbSet<WallPostLike> UserPostLikes { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<UniversityFaculty> Faculties { get; set; }
        public DbSet<UserPermissions> Permissionses { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<File> Files { get; set; }

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
