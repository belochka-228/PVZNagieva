using Api_Nagieva.Data;
using Api_Nagieva.DTO;
using Api_Nagieva.Models;
using Microsoft.EntityFrameworkCore;

namespace Api_Nagieva.Services
{
    public class ReviewService
    {
        private readonly MarketplaceDBContext _context;
        public ReviewService(MarketplaceDBContext context)
        {
            _context = context;
        }
        public async Task<List<ReviewResponse>> GetByProductIdAsync(int productId)
        {
            return await _context.Reviews.Include(r => r.User).Where(r => r.ProductId == productId).OrderByDescending(r => r.CreatedAt)
                .Select(r => new ReviewResponse(r.Id, r.ProductId, r.User.Login, r.Rating, r.Comment, r.CreatedAt)).ToListAsync();
        }
        public async Task<ReviewResponse> CreateAsync(int userId, CreateReviewRequest request)
        {
            var review = new Review
            {
                ProductId = request.ProductId,
                UserId = userId,
                Rating = request.Rating,
                Comment = request.Comment,
                CreatedAt = DateTime.Now
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            var user = await _context.Users.FindAsync(userId);
            return new ReviewResponse(review.Id, review.ProductId, user!.Login, review.Rating, review.Comment, review.CreatedAt);
        }
        public async Task<bool> DeleteAsync(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review == null) return false;

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
