using Api_Nagieva.DTO;
using Api_Nagieva.Models;
using Microsoft.EntityFrameworkCore;

namespace Api_Nagieva.Services
{
    public class FavoriteService
    {
        private readonly MarketplaceDBContext _context;

        public FavoriteService(MarketplaceDBContext context)
        {
            _context = context;
        }

        public async Task<List<FavoriteResponse>> GetFavoriteResponsesAsync(int userId)
        {
            return await _context.Favorites.Include(f => f.Product).Where(f => f.UserId == userId)
                .Select(f => new FavoriteResponse(f.ProductId, f.Product.Name, f.Product.Price, f.Product.ImagePath)).ToListAsync();
        }

        public async Task<bool> AddToFavoritesAsync(int userId, int productId)
        {
            var exists = await _context.Favorites.AnyAsync(f => f.UserId == userId && f.ProductId == productId);
            if (exists) 
                return true;

            _context.Favorites.Add(new Favorite { UserId = userId, ProductId = productId, CreatedAt = DateTime.Now });
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveFromFavoritesAsync(int userId, int productId)
        {
            var favorite = await _context.Favorites.FirstOrDefaultAsync(f => f.UserId == userId && f.ProductId == productId);
            if (favorite == null) 
                return false;

            _context.Favorites.Remove(favorite);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> MoveToCartAsync(int userId, int productId, CartService cartService)
        {
            var added = await cartService.AddToCartAsync(userId, new AddToCartRequest(productId, 1));
            if (!added) return false;

            await RemoveFromFavoritesAsync(userId, productId);
            return true;
        }

        internal async Task GetFavoritesAsync(int userId)
        {
            throw new NotImplementedException();
        }
    }
}
