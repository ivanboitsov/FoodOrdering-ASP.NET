using Microsoft.AspNetCore.Mvc;
using FoodOrderingWebsite.Services;
using System;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Principal;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using FoodOrderingWebsite.Models.User;
using FoodOrderingWebsite;

namespace FoodOrderingWebsite.Controllers
{
    [Route("api/account")]
    [ApiController]
    [AllowAnonymous]
    public class UsersController : ControllerBase
    {
        private readonly UserService _userService;

        public UsersController(UserService userService)
        {
            _userService = userService;
        }


        [HttpPost("register")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        public IActionResult Register([FromBody] UserEditModel user)
        {
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    return BadRequest("Вы уже авторизованы.");
                }

                if (_userService.UserExists(user.Email))
                {
                    return BadRequest("Пользователь с таким email уже существует");
                }

                if (_userService.GenderInvalid(user.Gender))
                {
                    return BadRequest("Гендер должен быть Male, либо Female");
                }

                if (!_userService.EmailValid(user.Email))
                {
                    return BadRequest("Некорректный емэил адрес");
                }

                if (_userService.PasswordInvalid(user.Password))
                {
                    return BadRequest("Пароль должен иметь длину от 8 символов");
                }

                if (!_userService.PhoneValid(user.Phone))
                {
                    return BadRequest($"Телефон '{user.Phone}' не соответствует маске +7 (xxx) xxx xx-xx");
                }

                string? dateStatus = _userService.checkBirthDateStatus(user.BirthDate);
                if (dateStatus != null)
                {
                    return BadRequest(dateStatus);
                }

                User createdUser = _userService.CreateUser(user);
                if (createdUser != null)
                {
                    AuthOptions authentification = new AuthOptions();

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, createdUser.Id.ToString())
                    };

                    var now = DateTime.UtcNow;
                    var jwt = new JwtSecurityToken(
                            issuer: AuthOptions.ISSUER,
                            audience: AuthOptions.AUDIENCE,
                            notBefore: now,
                            claims: claims,
                            expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
                            signingCredentials: new SigningCredentials(authentification.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
                    var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
                    _userService.CreateOrUpdateTokenInfo(createdUser.Id, encodedJwt);
                    var response = new
                    {
                        access_token = encodedJwt,
                        username = createdUser.FullName
                    };

                    return Ok(response);
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Произошла ошибка сервера");
            }
        }

        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        public IActionResult Login([FromBody] LoginCredentials model)
        {
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    return BadRequest("Вы уже авторизованы.");
                }

                User user = _userService.Authenticate(model.Email, model.Password);

                if (user == null)
                {
                    return BadRequest("Неверные учетные данные");
                }

                AuthOptions authentification = new AuthOptions();

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
                };

                var now = DateTime.UtcNow;
                var jwt = new JwtSecurityToken(
                        issuer: AuthOptions.ISSUER,
                        audience: AuthOptions.AUDIENCE,
                        notBefore: now,
                        claims: claims,
                        expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
                        signingCredentials: new SigningCredentials(authentification.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
                var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
                _userService.CreateOrUpdateTokenInfo(user.Id, encodedJwt);

                var response = new
                {
                    access_token = encodedJwt,
                    username = user.FullName,
                    id = user.Id,
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Произошла ошибка сервера" + "\n" + ex.ToString());
            }
        }

        [HttpPost("logout")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        public IActionResult Logout()
        {
            try
            {
                Guid userId = Guid.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value);

                string userStatus = _userService.CheckUserAuthorization(userId);
                if (userStatus == "notfound")
                {
                    return NotFound("Пользователь не найден");
                }
                if (userStatus == "unauthorized")
                {
                    return Unauthorized("Вы не вошли в аккаунт");
                }

                _userService.UnValidateToken(userId);

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Произошла ошибка сервера" + "\n" + ex.ToString());
            }
        }

        [HttpGet("profile")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        public IActionResult GetProfile()
        {
            try
            {
                Guid userId = Guid.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value);

                string userStatus = _userService.CheckUserAuthorization(userId);
                if (userStatus == "notfound")
                {
                    return NotFound("Пользователь не найден");
                }
                if (userStatus == "unauthorized")
                {
                    return Unauthorized("Вы не вошли в аккаунт");
                }

                if (userId != null)
                {
                    var user = _userService.GetUserById(userId);

                    if (!_userService.CheckCurrentToken(user.Id))
                    {
                        return Unauthorized();
                    }

                    if (user != null)
                    {
                        var response = new
                        {
                            id = user.Id,
                            email = user.Email,
                            fullName = user.FullName,
                            gender = user.Gender,
                            birthDate = user.BirthDate,
                            address = user.Address,
                            phoneNumber = user.Phone,
                        };

                        return Ok(response);
                    }
                    else
                    {
                        return NotFound("Пользователь не найден");
                    }
                }
                else
                {
                    return NotFound("Id не найден");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Произошла ошибка сервера");
            }
        }

        [HttpPut("profile")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        public IActionResult UpdateProfile([FromBody] UserEditModel updatedUser)
        {
            try
            {
                Guid userId = Guid.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value);

                string userStatus = _userService.CheckUserAuthorization(userId);
                if (userStatus == "notfound")
                {
                    return NotFound("Пользователь не найден");
                }
                if (userStatus == "unauthorized")
                {
                    return Unauthorized("Вы не вошли в аккаунт");
                }

                if (userId != null)
                {
                    var user = _userService.GetUserById(userId);

                    if (!_userService.CheckCurrentToken(user.Id))
                    {
                        return Unauthorized();
                    }

                    if (user != null)
                    {

                        if (_userService.GenderInvalid(updatedUser.Gender))
                        {
                            return BadRequest("Гендер должен быть Male, либо Female");
                        }

                        if (!_userService.PhoneValid(user.Phone))
                        {
                            return BadRequest($"Телефон '{user.Phone}' не соответствует маске +7 (xxx) xxx xx-xx");
                        }

                        string? dateStatus = _userService.checkBirthDateStatus(user.BirthDate);
                        if (dateStatus != null)
                        {
                            return BadRequest(dateStatus);
                        }

                        user.Address = updatedUser.Address;
                        user.Phone = updatedUser.Phone;
                        user.Gender = updatedUser.Gender;
                        user.FullName = updatedUser.FullName;
                        user.BirthDate = updatedUser.BirthDate;

                        _userService.UpdateUser(user);

                        return Ok();
                    }
                    else
                    {
                        return NotFound("Пользователь не найден");
                    }
                }
                else
                {
                    return BadRequest("Email не найден");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Произошла ошибка сервера");
            }
        }
    }
}
