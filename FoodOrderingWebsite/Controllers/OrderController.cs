using FoodOrderingWebsite.Models;
using FoodOrderingWebsite.Services;
using FoodOrderingWebsite.Models.Order;
using FoodOrderingWebsite.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FoodOrderingWebsite.Controllers
{
    [Route("api/order")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly OrderService _orderService;
        private readonly UserService _userService;

        public OrderController(OrderService orderService, UserService userService)
        {
            _orderService = orderService;
            _userService = userService;
        }

        [HttpGet("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OrderResponseModel))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        public IActionResult GetConcreteOrder(Guid id)
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

                OrderResponseModel order = _orderService.GetOrderById(id);
                if (order != null)
                {
                    return Ok(order);
                }
                else
                {
                    return NotFound("Заказ не найден");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Произошла ошибка сервера");
            }
        }

        [HttpGet("")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<OrderResponseModel>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        public IActionResult GetOrders()
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

                List<OrderResponseModel> orders = _orderService.GetOrders();
                if (orders != null)
                {
                    return Ok(orders);
                }
                else
                {
                    return NotFound("Заказы не найдены");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Произошла ошибка сервера");
            }
        }

        [HttpPost("")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OrderResponseModel))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        public IActionResult CreateOrder([FromBody] CreateOrderModel rawOrder)
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

                if (!_orderService.IsOrderTimeValid(rawOrder.deliveryTime))
                {
                    return BadRequest("Время доставки должно быть не раньше, чем через 30 минут от текущего времени.");
                }

                OrderResponseModel result = _orderService.CreateOrder(rawOrder, userId);
                if (result == null)
                {
                    return BadRequest("невозможно сформировать пустой заказ");
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Произошла ошибка сервера");
            }
        }

        [HttpPost("{id}/status")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OrderResponseModel))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        public IActionResult ConfirmOPrder(Guid id)
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

                var response = _orderService.ConfirmOrder(id, userId);
                if (response == "ok")
                {
                    OrderResponseModel order = _orderService.GetOrderById(id);
                    return Ok(order);
                }
                else if (response == "notfound")
                {
                    return NotFound("Заказ не найден");
                }
                else if (response == "forbidden")
                {
                    return Forbid("У вас нет прав подтверждать этот заказ");
                }
                else if (response == "alreadyConfirmed")
                {
                    return Forbid("Этот заказ уже подтверждён");
                }
                return BadRequest("Неизвестная ошибка запроса");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Произошла ошибка сервера");
            }
        }
    }
}
