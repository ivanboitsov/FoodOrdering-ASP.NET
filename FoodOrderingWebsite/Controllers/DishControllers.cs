using FoodOrderingWebsite.Models;
using FoodOrderingWebsite.Services;
using FoodOrderingWebsite.Models.Dish;
using FoodOrderingWebsite.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Security.Claims;
using System.Text.Json.Serialization;
using FoodOrderingWebsite.Models.Order;

namespace FoodOrderingWebsite.Controllers
{
    [Route("api/dish")]
    [ApiController]
    public class DishController : ControllerBase
    {
        private readonly DishService _dishService;
        private readonly UserService _userService;

        public DishController(DishService dishService, UserService userService)
        {
            _dishService = dishService;
            _userService = userService;
        }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum DishCategory
        {
            All,
            Wok,
            Pizza,
            Soup,
            Dessert,
            Drink
        }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum DishVegetarian
        {
            NoMatter,
            True,
            False
        }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum DishSorting
        {
            NameAsc,
            NameDesc,
            PriceAsc,
            PriceDesc,
            RatingAsc,
            RatingDesc
        }

        [HttpGet("dishes")]
        public IActionResult GetDishes(
        [FromQuery] DishCategory? category = null,
        [FromQuery] DishVegetarian? vegetarian = null,
        [FromQuery] DishSorting? sorting = null,
        [FromQuery] int page = 1)
        {
            try
            {
                if (category.ToString() == "" || vegetarian.ToString() == "" || sorting.ToString() == "")
                {
                    return BadRequest("Пожалуйста, выберите значения для всех параметров.");
                }
                var result = _dishService.GetAllDishes(category.ToString(), vegetarian.ToString(), sorting.ToString(), page);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Произошла ошибка сервера " + ex.ToString());
            }
        }

        [HttpGet("dish/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OrderResponseModel))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        public IActionResult GetDishId(string id)
        {
            try
            {
                Dish result = _dishService.GetDishById(id);

                if (result == null)
                {
                    return BadRequest("Неправильный Id");
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Произошла ошибка сервера " + ex.ToString());
            }
        }

        [HttpGet("dish/{id}/raiting/check")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OrderResponseModel))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        public IActionResult CheckDishRaiting(string id)
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

                Dish dish = _dishService.GetDishById(id);

                if (dish == null)
                {
                    return NotFound("Блюдо с данным Id не существует");
                }

                if (!_dishService.IsUserAbleToPostRaiting(dish, userId))
                {
                    return Forbid("false\nВы должны иметь хотя бы 1 заказ с этим блюдом, чтобы иметь возможность его оценить.");
                }

                return Ok(true);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Произошла ошибка сервера " + ex.ToString());
            }
        }

        [HttpPost("dish/{id}/raiting")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OrderResponseModel))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        public IActionResult PostDishRaiting(string id, int raitingScore)
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

                if (raitingScore < 0 || raitingScore > 5)
                {
                    return BadRequest("Значение рейтинга должно быть в пределах от 0 до 5");
                }

                Dish dish = _dishService.GetDishById(id);

                if (dish == null)
                {
                    return NotFound("Блюдо с заданным Id не существует");
                }

                if (!_dishService.IsUserAbleToPostRaiting(dish, userId))
                {
                    return Forbid("Вы должны иметь хотя бы 1 заказ с этим блюдом, чтобы иметь возможность его оценить.");
                }

                _dishService.PostDishRaiting(dish, userId, raitingScore);
                _dishService.UpdateDishRaiting(dish);

                return Ok("Рейтинг Блюда обновлен");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Произошла ошибка сервера");
            }
        }
    }
}
