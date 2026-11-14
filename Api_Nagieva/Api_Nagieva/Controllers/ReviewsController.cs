using Api_Nagieva.DTO;
using Api_Nagieva.Models;
using Api_Nagieva.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api_Nagieva.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewsController : ControllerBase
    {
        private readonly ReviewService _reviewService;

        public ReviewsController(ReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpGet("product/{productId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByProduct(int productId)
        {
            var reviews = await _reviewService.GetByProductIdAsync(productId);
            return Ok(reviews);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(CreateReviewRequest request)
        {
            if (!int.TryParse(User.FindFirst("sub")?.Value, out int userId))
                return Unauthorized();

            var review = await _reviewService.CreateAsync(userId, request);
            return Ok(review);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _reviewService.DeleteAsync(id);
            if (!success) return NotFound("Отзыв не найден");
            return Ok("Отзыв удален");
        }
    }
}
