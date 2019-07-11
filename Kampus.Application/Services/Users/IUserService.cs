﻿using Kampus.Models;
using Kampus.Persistence.Enums;
using System.Collections.Generic;

namespace Kampus.Application.Services.Users
{
    public interface IUserService
    {
        List<UserModel> GetAll();
        UserModel GetById(int userId);
        UserModel GetByUsername(string username);
        void RegisterUser(UserModel model);
        SignInResult SignIn(string username, string password);
        bool ContainsUserWithSuchUsername(string username);
        bool ContainsUserWithSuchEmail(string email);
        void SetAvatar(int userId, string path);
        void ChangePassword(int userId, string oldPassword, string newPassword, string newPasswordConfirm);
        void ChangeStatus(int userId, string status);
        void ChangeStudentInfo(int userId, string university, string faculty, int course);
        void ChangeCity(int userId, string city);
    }
}