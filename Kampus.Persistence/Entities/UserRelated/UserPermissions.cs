namespace Kampus.Persistence.Entities.UserRelated
{
    public class UserPermissions
    {
        public int UserPermissionId { get; set; }
        public bool AllowToWriteOnMyWall { get; set; }
        public bool AllowToWriteComments { get; set; }
        public bool AllowToSendMeAMessage { get; set; }
    }
}
