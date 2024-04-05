using System.Net.Mail;
using System.Text.RegularExpressions;
using System;
using FoodOrderingWebsite.Models.User;
using FoodOrderingWebsite.Data;
using FoodOrderingWebsite.Models;

namespace FoodOrderingWebsite.Services
{
    public class UserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public string CheckUserAuthorization(Guid UserId)
        {
            User user = GetUserById(UserId);
            if (user != null)
            {
                if (CheckCurrentToken(UserId))
                {
                    return "ok";
                }
                return "unauthorized";
            }
            return "notfound";
        }

        public bool UserExists(string email)
        {
            return _context.Users.Any(u => u.Email == email);
        }

        public bool GenderInvalid(string gender)
        {
            return gender != "Male" && gender != "Female";
        }

        public bool EmailValid(string email)
        {
            try
            {
                var mailAddress = new MailAddress(email);
                return mailAddress.Address == email;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        public bool PhoneValid(string phone)
        {
            string pattern = @"^\+7 \(\d{3}\) \d{3}[- ]\d{2}[- ]\d{2}$";
            Regex regex = new Regex(pattern);
            return regex.IsMatch(phone);
        }

        public bool PasswordInvalid(string password)
        {
            return password.Length < 8;
        }

        public string? checkBirthDateStatus(DateTime date)
        {
            DateTime currentDate = DateTime.Now;
            DateTime earliestDate = currentDate.AddYears(-120);
            DateTime latestDate = currentDate.AddYears(-3);
            if (date < earliestDate)
            {
                return "Ошибка с возрастом?";
            }
            else if (date > latestDate)
            {
                return "Ошибка с возрастом?";
            }
            return null;
        }

        public void CreateOrUpdateTokenInfo(Guid UserId, string token)
        {
            TokenInfo tokenInfo = _context.Tokens.FirstOrDefault(x => x.UserId == UserId);
            if (tokenInfo == null)
            {
                tokenInfo = new TokenInfo
                {
                    UserId = UserId,
                    Token = token,
                    CreateTime = DateTime.UtcNow,
                    IsValid = true,
                };
                _context.Tokens.Add(tokenInfo);
                _context.SaveChanges();
            }
            else
            {
                tokenInfo.Token = token;
                tokenInfo.CreateTime = DateTime.UtcNow;
                tokenInfo.IsValid = true;
                _context.Tokens.Update(tokenInfo);
                _context.SaveChanges();
            }
        }

        public void UnValidateToken(Guid UserId)
        {
            TokenInfo tokenInfo = _context.Tokens.FirstOrDefault(x => x.UserId == UserId);
            tokenInfo.IsValid = false;
            _context.Tokens.Update(tokenInfo);
            _context.SaveChanges();
        }

        public bool IsTokenValid(Guid UserId, string token)
        {
            TokenInfo tokenInfo = _context.Tokens.FirstOrDefault(x => x.UserId == UserId);
            if (tokenInfo == null)
            {
                return false;
            }
            if (tokenInfo.Token != token)
            {
                tokenInfo.IsValid = false;
            }
            if (tokenInfo.CreateTime.AddMinutes(AuthOptions.LIFETIME) < DateTime.Now)
            {
                tokenInfo.IsValid = false;
            }

            _context.Tokens.Update(tokenInfo);
            _context.SaveChanges();
            return tokenInfo.IsValid;
        }

        public bool CheckCurrentToken(Guid UserId)
        {
            TokenInfo tokenInfo = _context.Tokens.FirstOrDefault(x => x.UserId == UserId);
            if (tokenInfo == null)
            {
                return false;
            }
            return tokenInfo.IsValid;
        }

        public User GetUserById(string id)
        {
            return _context.Users.FirstOrDefault(u => u.Id.ToString() == id);
        }

        public User Authenticate(string email, string password)
        {
            User user = _context.Users.SingleOrDefault(u => u.Email == email);

            if (user == null)
            {
                return null;
            }

            Password userPassword = _context.Passwords.SingleOrDefault(p => p.UserId == user.Id);

            if (userPassword == null)
            {
                return null;
            }
            else if (!userPassword.PasswordValue.Equals(password))
            {
                return null;
            }

            return user;
        }

        public User? CreateUser(UserEditModel user)
        {
            Guid userId = Guid.NewGuid();
            User fullUser = new User
            {
                Id = userId,
                Email = user.Email,
                Phone = user.Phone,
                Address = user.Address,
                BirthDate = user.BirthDate,
                Gender = user.Gender,
                FullName = user.FullName
            };

            Password userPassword = new Password
            {
                UserId = userId,
                PasswordValue = user.Password,
            };

            _context.Users.Add(fullUser);
            _context.Passwords.Add(userPassword);
            _context.SaveChanges();
            return fullUser;
        }

        public User GetUserById(Guid id)
        {
            return _context.Users.FirstOrDefault(u => u.Id == id);
        }

        public List<User> GetAllUsers()
        {
            return _context.Users.ToList();
        }

        public User UpdateUser(User user)
        {
            _context.Users.Update(user);
            _context.SaveChanges();
            return user;
        }

        public void DeleteUser(Guid id)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == id);
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
            }
        }
    }
}
