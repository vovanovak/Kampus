namespace Kampus.Models
{
    public class UserShortModel
    {
        public int? Id { get; set; }
        public string Username { get; set; }
        public string Avatar { get; set; }

        public UserShortModel()
        {
        }

        public static UserShortModel From(UserModel model)
        {
            UserShortModel u = new UserShortModel();
            u.Id = model.Id;
            u.Username = model.Username;
            u.Avatar = model.Avatar;
            return u;
        }
        public static UserShortModel From(int? id, string username, string avatar)
        {
            UserShortModel u = new UserShortModel();
            u.Id = id;
            u.Username = username;
            u.Avatar = avatar;
            return u;
        }
    }
}