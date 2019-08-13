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

        public UserShortModel(int? id, string username, string avatar)
        {
            Id = id;
            Username = username;
            Avatar = avatar;
        }

        public static UserShortModel From(UserModel model)
        {
            return new UserShortModel(model.Id, model.Username, model.Avatar);
        }
    }
}