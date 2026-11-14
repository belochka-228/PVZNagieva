using Api_Nagieva.DTO;
using Api_Nagieva.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace Api_Nagieva.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]

    public class CartController : ControllerBase
    {
        private readonly CartService _cartService;

        public CartController(CartService cartService)
        {
            _cartService = cartService;
        }

        private int GetUserId()
        {
            int.TryParse(User.FindFirst("sub")?.Value, out int userId);
            return userId;
        }

        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var userId = GetUserId();
            var cart = await _cartService.GetCartAsync(userId);
            return Ok(cart);
        }

        [HttpPost("add")]
        public async Task<IActionResult> Add([FromBody] AddToCartRequest request)
        {
            var userId = GetUserId();
            var success = await _cartService.AddToCartAsync(userId, request);

            if (!success) return BadRequest("Товара нет на складе или он недоступен.");
            return Ok("Товар добавлен в корзину");
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update([FromBody] UpdateCartQuantityRequest request)
        {
            var userId = GetUserId();
            var success = await _cartService.UpdateQuantityAsync(userId, request);

            if (!success) return BadRequest("Не удалось обновить количество (возможно, не хватает на складе).");
            return Ok("Количество обновлено");
        }

        [HttpDelete("{productId}")]
        public async Task<IActionResult> Remove(int productId)
        {
            var userId = GetUserId();
            var success = await _cartService.RemoveFromCartAsync(userId, productId);

            if (!success) return NotFound("Товара нет в корзине");
            return Ok("Товар удален");
        }

    }
}
