using Api_Nagieva.Models;
using Api_Nagieva.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api_Nagieva.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FavoritesController : ControllerBase
    {
        private readonly FavoriteService _favoriteService;
        private readonly CartService _cartService;

        public FavoritesController(FavoriteService favoriteService, CartService cartService)
        {
            _favoriteService = favoriteService;
            _cartService = cartService;
        }

        private int GetUserId()
        {
            int.TryParse(User.FindFirst("sub")?.Value, out int userId);
            return userId;
        }

        [HttpGet]
        public async Task<IActionResult> GetFavorites()
        {
            var userId = GetUserId();
            var favorites = await _favoriteService.GetFavoriteResponsesAsync(userId);
            return Ok(favorites);
        }

        [HttpPost("{productId}")]
        public async Task<IActionResult> Add(int productId)
        {
            var userId = GetUserId();
            await _favoriteService.AddToFavoritesAsync(userId, productId);
            return Ok("Добавлено в избранное");
        }

        [HttpDelete("{productId}")]
        public async Task<IActionResult> Remove(int productId)
        {
            var userId = GetUserId();
            var success = await _favoriteService.RemoveFromFavoritesAsync(userId, productId);

            if (!success) return NotFound("Товара нет в избранном");
            return Ok("Удалено из избранного");
        }

        [HttpPost("{productId}/move-to-cart")]
        public async Task<IActionResult> MoveToCart(int productId)
        {
            var userId = GetUserId();
            var success = await _favoriteService.MoveToCartAsync(userId, productId, _cartService);

            if (!success) return BadRequest("Не удалось перенести в корзину (товар недоступен).");
            return Ok("Перенесено в корзину");
        }
    }
}
