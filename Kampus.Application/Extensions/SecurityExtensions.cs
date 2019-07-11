using System;
using System.Security.Cryptography;
using System.Text;

namespace Kampus.Application.Extensions
{
    public static class SecurityExtensions
    {
        public static string GetEncodedHash(this string path)
        {
            const string salt = "adhasdhasdhas";
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] digest = md5.ComputeHash(Encoding.UTF8.GetBytes(path + salt));
            string base64digest = Convert.ToBase64String(digest, 0, digest.Length);
            return base64digest.Substring(0, base64digest.Length - 2);
        }
    }
}
