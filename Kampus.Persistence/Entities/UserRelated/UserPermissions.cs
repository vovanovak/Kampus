namespace Kampus.Persistence.Entities.UserRelated
{
    public class UserPermissions
    {
        public int UserPermissionsId { get; set; }
        public bool AllowToWriteOnMyWall { get; set; }
        public bool AllowToWriteComments { get; set; }
        public bool AllowToSendMeAMessage { get; set; }

        public static UserPermissions AllowAll()
        {
            return new UserPermissions
            {
                AllowToWriteComments = true,
                AllowToWriteOnMyWall = true,
                AllowToSendMeAMessage = true,
            };
        }
    }
}