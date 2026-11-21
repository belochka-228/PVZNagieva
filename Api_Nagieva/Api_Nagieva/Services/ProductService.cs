using Api_Nagieva.DTO;
using Api_Nagieva.Models;
using Microsoft.EntityFrameworkCore;

namespace Api_Nagieva.Services
{
    public class ProductService
    {
        private readonly MarketplaceDBContext _context;

        public ProductService(MarketplaceDBContext context)
        {
            _context = context;
        }
        public async Task<List<ProductResponse>> GetAllAsync(string? search, int? categoryId, decimal? minPrice, decimal? maxPrice)
        {
            var query = _context.Products.Include(p => p.Category).Include(p => p.Reviews).AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(p => p.Name.Contains(search));
            if (categoryId.HasValue)
                query = query.Where(p => p.CategoryId == categoryId.Value);
            if (minPrice.HasValue)
                query = query.Where(p => p.Price >= minPrice.Value);
            if (maxPrice.HasValue)
                query = query.Where(p => p.Price <= maxPrice.Value);

            return await query.Select(p => new ProductResponse(
                p.Id, p.Name, p.Description, p.Price, p.ImagePath,
                p.CategoryId, p.Category.Name,
                p.StockQuantity, p.PackQuantity, p.IsAvailable && p.StockQuantity > 0,
                p.Reviews.Any() ? p.Reviews.Average(r => r.Rating) : 0
            )).ToListAsync();
        }
        public async Task<ProductResponse?> GetByIdAsync(int id)
        {
            return await _context.Products.Include(p => p.Category).Include(p => p.Reviews).Where(p => p.Id == id)
                .Select(p => new ProductResponse(p.Id, p.Name, p.Description, p.Price, p.ImagePath, p.CategoryId, p.Category.Name, p.StockQuantity,
                    p.PackQuantity, p.IsAvailable && p.StockQuantity > 0, p.Reviews.Any() ? p.Reviews.Average(r => r.Rating) : 0)).FirstOrDefaultAsync();
        }
        public async Task<ProductResponse> CreateAsync(CreateProductRequest request)
        {
            var product = new Product
            {
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                ImagePath = request.ImagePath,
                CategoryId = request.CategoryId,
                StockQuantity = request.StockQuantity,
                PackQuantity = request.PackQuantity,
                IsAvailable = request.IsAvailable
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return (await GetByIdAsync(product.Id))!;
        }
        public async Task<bool> UpdateAsync(int id, CreateProductRequest request)
        {
            Console.WriteLine($"ОБНОВЛЕНИЕ ТОВАРА {id}: Пришел CategoryId = {request.CategoryId}");

            var product = await _context.Products.FindAsync(id);
            if (product == null) return false;

            product.Name = request.Name;
            product.Description = request.Description;
            product.Price = request.Price;
            product.ImagePath = request.ImagePath;
            product.CategoryId = request.CategoryId;
            product.StockQuantity = request.StockQuantity;
            product.PackQuantity = request.PackQuantity;
            product.IsAvailable = request.IsAvailable;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
