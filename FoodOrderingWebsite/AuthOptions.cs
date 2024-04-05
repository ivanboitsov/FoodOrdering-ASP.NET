using System;
using System.IO;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace FoodOrderingWebsite
{
    public class AuthOptions
    {
        public const string ISSUER = "MyAuthServer"; // издатель токена
        public const string AUDIENCE = "MyAuthClient"; // потребитель токена
        string KEY = Environment.GetEnvironmentVariable("SEED_KEY");   // ключ для шифрации
        public const int LIFETIME = 1; // время жизни токена - 1 минута


        public SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
        }
    }
}