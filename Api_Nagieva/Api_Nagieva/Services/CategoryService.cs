using Api_Nagieva.DTO;
using Api_Nagieva.Models;
using Microsoft.EntityFrameworkCore;

namespace Api_Nagieva.Services
{
    public class CategoryService
    {
        private readonly MarketplaceDBContext _context;
        public CategoryService(MarketplaceDBContext context)
        {
            _context = context;
        }
        public async Task<List<CategoryResponse>> GetAllAsync()
        {
            return await _context.Categories.Select(c => new CategoryResponse(c.Id, c.Name)).ToListAsync();
        }
        public async Task<CategoryResponse> CreateAsync(CreateCategoryRequest request)
        {
            var category = new Category { Name = request.Name };
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return new CategoryResponse(category.Id, category.Name);
        }
    }
}
