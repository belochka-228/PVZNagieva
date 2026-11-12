using Api_Nagieva.Data;
using Api_Nagieva.DTO;
using Api_Nagieva.Models;
using Microsoft.EntityFrameworkCore;

namespace Api_Nagieva.Services
{
    public class CartService
    {
        private readonly MarketplaceDBContext _context;

        public CartService(MarketplaceDBContext context)
        {
            _context = context;
        }

        public async Task<List<CartItemResponse>> GetCartAsync(int userId)
        {
            return await _context.CartItems.Include(c => c.Product).Where(c => c.UserId == userId)
                .Select(c => new CartItemResponse(c.ProductId, c.Product.Name, c.Product.Price, c.Quantity,
                    c.Product.Price * c.Quantity, c.Product.ImagePath)).ToListAsync();
        }

        public async Task<bool> AddToCartAsync(int userId, AddToCartRequest request)
        {
            var product = await _context.Products.FindAsync(request.ProductId);
            if (product == null || !product.IsAvailable || product.StockQuantity < request.Quantity)
                return false;

            var cartItem = await _context.CartItems.FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == request.ProductId);

            if (cartItem != null)
            {
                cartItem.Quantity += request.Quantity;
            }
            else
            {
                _context.CartItems.Add(new CartItem
                {
                    UserId = userId,
                    ProductId = request.ProductId,
                    Quantity = request.Quantity
                });
            }
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateQuantityAsync(int userId, UpdateCartQuantityRequest request)
        {
            var cartItem = await _context.CartItems.Include(c => c.Product).FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == request.ProductId);

            if (cartItem == null || cartItem.Product.StockQuantity < request.Quantity)
                return false;

            if (request.Quantity <= 0)
            {
                _context.CartItems.Remove(cartItem);
            }
            else
            {
                cartItem.Quantity = request.Quantity;
            }
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveFromCartAsync(int userId, int productId)
        {
            var cartItem = await _context.CartItems.FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == productId);
            if (cartItem == null) 
                return false;

            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
