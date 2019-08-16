using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Kampus.Models
{
    public partial class UserModel : Entity
    {
        [Required(ErrorMessage = "Введіть username", AllowEmptyStrings = false)]
        public string Username { get; set; }

        [Required(ErrorMessage = "Введіть пароль", AllowEmptyStrings = false)]
        [DataType(System.ComponentModel.DataAnnotations.DataType.Password)]
        public string Password { get; set; }

        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Введені паролі не збігаються")]
        [DataType(System.ComponentModel.DataAnnotations.DataType.Password)]
        [Required(ErrorMessage = "Підтвердіть пароль", AllowEmptyStrings = false)]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Введіть ім'я і прізвище", AllowEmptyStrings = false)]
        public string FullName { get; set; }

        [RegularExpression(
            @"^([0-9a-zA-Z]([\+\-_\.][0-9a-zA-Z]+)*)+@(([0-9a-zA-Z][-\w]*[0-9a-zA-Z]*\.)+[a-zA-Z0-9]{2,3})$",
            ErrorMessage = "Введіть валідний e-mail")]
        [Required(ErrorMessage = "Введіть e-mail", AllowEmptyStrings = false)]
        public string Email { get; set; }

        [DataType(System.ComponentModel.DataAnnotations.DataType.PhoneNumber)]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Введіть дату народження", AllowEmptyStrings = false)]
        [DataType(System.ComponentModel.DataAnnotations.DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "Виберіть місто", AllowEmptyStrings = false)]
        public string City { get; set; }

        [DataType(System.ComponentModel.DataAnnotations.DataType.ImageUrl)]
        public string Avatar { get; set; }

        public string UniversityName { get; set; }
        public string UniversityFaculty { get; set; }

        [Range(1, 6, ErrorMessage = "Введіть правильне значення курсу")]
        public int? UniversityCourse { get; set; }

        public bool IsNotStudent { get; set; }

        public string UserRole { get; set; }

        public string Status { get; set; }
        public string About { get; set; }

        public double Rating { get; set; }

        public virtual List<string> Achievements { get; set; }

        public virtual List<UserShortModel> Friends { get; set; }
        public virtual List<UserShortModel> Subscribers { get; set; }
        public virtual List<TaskModel> Tasks { get; set; }
        public virtual List<MessageModel> Messages { get; set; }
        public virtual IReadOnlyList<WallPostModel> Posts { get; set; }
        public virtual List<GroupModel> Groups { get; set; }
    }
}