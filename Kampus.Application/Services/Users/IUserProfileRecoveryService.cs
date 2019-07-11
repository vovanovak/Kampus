using Kampus.Persistence.Entities.UserRelated;

namespace Kampus.Application.Services.Users
{
    public interface IUserProfileRecoveryService
    {
        void SendRecoveryLetter(UserRecovery recovery, string path);
        void RecoverPassword(string username, string email, string path);
        string ContainsRecoveryWithSuchHash(string str);
    }
}
