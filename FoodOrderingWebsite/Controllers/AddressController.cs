using FoodOrderingWebsite.Models.Order;
using FoodOrderingWebsite.Services;
using Microsoft.AspNetCore.Mvc;

namespace FoodOrderingWebsite.Controllers
{
    [Route("api/address")]
    [ApiController]
    public class AddressController : ControllerBase
    {
        private readonly AddressService _addressService;

        public AddressController(AddressService addressService)
        {
            _addressService = addressService;
        }

        [HttpGet("search")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OrderResponseModel))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        public IActionResult Search(int parentObjectId, string? query)
        {
            try
            {
                var response = _addressService.SearchAddress(parentObjectId, query);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Произошла ошибка сервера " + ex.ToString());
            }
        }

        [HttpGet("chain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OrderResponseModel))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        public IActionResult SearchChain(string objectGuid)
        {
            try
            {
                var response = _addressService.SearchAddressChain(Guid.Parse(objectGuid));
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Произошла ошибка сервера " + ex.ToString());
            }
        }
    }
}
