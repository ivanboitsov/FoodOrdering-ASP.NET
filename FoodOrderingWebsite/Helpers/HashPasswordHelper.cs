// Хеширование взято с этого видео https://www.youtube.com/watch?v=Z0kB01kc038
using System;
using System.Security.Cryptography;
using System.Text;

namespace FoodOrderingWebsite.Helpers
{
    public static class HashPasswordHelper
    {
        // Хеширование пароля происходит с помощью алгоритма sha256
        public static string HashPassword(string password)
        {
            using(var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                var hash = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();

                return hash;
            }
        }
    }
}
