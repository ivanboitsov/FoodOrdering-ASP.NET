using FoodOrderingWebsite.Models.Basket;
using FoodOrderingWebsite.Models.Order;
using FoodOrderingWebsite.Models.User;
using FoodOrderingWebsite.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FoodOrderingWebsite.Controllers
{
    [Route("api/basket")]
    [ApiController]
    public class BasketController : ControllerBase
    {
        private readonly BasketService _basketService;
        private readonly UserService _userService;

        public BasketController(BasketService basketService, UserService userService)
        {
            _basketService = basketService;
            _userService = userService;
        }

        [HttpGet("")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OrderResponseModel))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        public IActionResult Search()
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

                List<BasketResponseModel> response = _basketService.GetAllBaskets();
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Произошла ошибка сервера " + ex.ToString());
            }
        }

        [HttpPost("dish/{dishId}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OrderResponseModel))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        public IActionResult AddDish(string dishId)
        {
            try
            {
                Guid dishGuidId = Guid.Parse(dishId);
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

                string result = _basketService.AddDishToBasket(userId, dishGuidId);

                if (result == "notfound")
                {
                    return NotFound("Блюдо с таким ID не существует");
                }

                return Ok("Блюдо добавлено в корзину");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Произошла ошибка сервера " + ex.ToString());
            }
        }

        [HttpDelete("dish/{dishId}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OrderResponseModel))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        public IActionResult DeleteDish(string dishId)
        {
            try
            {
                Guid dishGuidId = Guid.Parse(dishId);
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

                string result = _basketService.DeleteDishFromBasket(userId, dishGuidId);
                if (result == "notfound")
                {
                    return NotFound("Данное блюдо отсутствует в корзине");
                }

                return Ok("Блюдо убрано из корзины");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Произошла ошибка сервера " + ex.ToString());
            }
        }
    }
}
