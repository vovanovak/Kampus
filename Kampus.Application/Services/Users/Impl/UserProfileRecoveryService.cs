using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using Kampus.Application.Extensions;
using Kampus.Persistence.Contexts;
using Kampus.Persistence.Entities.UserRelated;

namespace Kampus.Application.Services.Users.Impl
{
    public class UserProfileRecoveryService : IUserProfileRecoveryService
    {
        private readonly KampusContext _context;

        public UserProfileRecoveryService(KampusContext context)
        {
            _context = context;
        }

        public string ContainsRecoveryWithSuchHash(string str)
        {
            return _context.Recoveries.FirstOrDefault(r => r.HashString == str)?.User.Username;
        }

        public void RecoverPassword(string username, string email, string path)
        {
            User user = _context.Users.First(u => u.Username == username);

            if (user.Email == email)
            {
                UserRecovery recovery = new UserRecovery();
                recovery.User = user;
                recovery.UserId = user.Id;
                recovery.HashString = (DateTime.Now.Ticks.ToString() + username + email + path).GetEncodedHash();

                SendRecoveryLetter(recovery, path);

                _context.Recoveries.Add(recovery);

                _context.SaveChanges();
            }
        }

        public void SendRecoveryLetter(UserRecovery recovery, string path)
        {
            var fromAddress = new MailAddress("cinemaserverrivne@gmail.com", "Kampus.com");
            var toAddress = new MailAddress(recovery.User.Email, "");
            string fromPassword = "cinemaserverrivne1";
            string subject = "Відновлення пароля";
            string body = "Відновіть пароль перейшовши за цим посиланням: " + path + recovery.HashString;

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };
            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            })
            {
                smtp.Send(message);
            }
        }
    }
}
