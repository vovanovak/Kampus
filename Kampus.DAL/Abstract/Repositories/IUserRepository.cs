using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using Kampus.Entities;
using Kampus.Models;
using Kampus.DAL.Enums;

namespace Kampus.DAL.Abstract.Repositories
{
    public interface IUserRepository: IRepository<UserModel>
    {
        UserModel GetByUsername(string username);
        void RegisterUser(UserModel model);
        SignInResult SignIn(string username, string password);
        List<UserShortModel> GetUserSubscribers(int userId);
        List<UserShortModel> GetUserFriends(int userId);
        void AddFriend(int id, int userId);
        void AddSubscriber(UserModel user, UserModel sender);
        bool ContainsUserWithSuchUsername(string username);
        void RemoveFriend(int id, int friendid);
        void SendRecoveryLetter(UserRecovery recovery, string path);
        void RecoverPassword(string username, string email, string path);
        string ContainsRecoveryWithSuchHash(string str);
        void SetNewPassword(string username, string password);
        void SetAvatar(int userId, string path);
        void ChangePassword(int userId, string oldpassword, string newpassword, string newpasswordconfirm);
        void ChangeStatus(int userId, string status);
        void ChangeStudentInfo(int userId, string university, string faculty, int course);
        void ChangeCity(int userId, string city);
        int CalculateAge(DateTime bday);
        UserSearchModel UpdateUserSearch(string request, string university, string faculty,
            string city, int? course, int? minAge, int? maxAge, int? minRating, int? maxRating);
        List<UserModel> SearchUsers(string request, string university, string faculty,
            string city, int? course, int? minAge, int? maxAge, int? minRating, int? maxRating);
        bool ContainsUserWithSuchEmail(string email);
    }
}