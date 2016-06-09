using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using Kampus.Entities;
using Kampus.Models;

namespace Kampus.DAL.Abstract
{
    public interface IUserRepository: IRepository<UserModel>
    {
        UserModel GetByUsername(string username);
        void RegisterUser(UserModel model);
        SignInResult SignIn(string username, string password);
        List<UserShortModel> GetUserSubscribers(int userid);
        List<UserShortModel> GetUserFriends(int userid);
        void AddFriend(int id, int userid);
        void AddSubscriber(UserModel user, UserModel sender);
        bool ContainsUserWithSuchUsername(string username);
        void RemoveFriend(int id, int friendid);
        string GetEncodedHash(string path);
        void SendRecoveryLetter(UserRecovery recovery, string path);
        void RecoverPassword(string username, string email, string path);
        string ContainsRecoveryWithSuchHash(string str);
        void SetNewPassword(string username, string password);
        void SetAvatar(int userid, string path);
        void ChangePassword(int userid, string oldpassword, string newpassword, string newpasswordconfirm);
        void ChangeStatus(int userid, string status);
        void ChangeStudentInfo(int userid, string university, string faculty, int course);
        void ChangeCity(int userid, string city);
        int CalculateAge(DateTime bday);
        UserSearchModel UpdateUserSearch(string request, string university, string faculty,
            string city, int? course, int? minage, int? maxage, int? minrating, int? maxrating);
        List<UserModel> SearchUsers(string request, string university, string faculty,
            string city, int? course, int? minage, int? maxage, int? minrating, int? maxrating);
    }
}