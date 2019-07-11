using Kampus.Persistence.Entities.UserRelated;

namespace Kampus.Application.Services.Users
{
    public interface IUserProfileRecoveryService
    {
        void SendRecoveryLetter(UserRecovery recovery, string path);
        void RecoverPassword(string username, string email, string path);
        void SetNewPassword(string username, string password);
        string ContainsRecoveryWithSuchHash(string str);
    }
}
