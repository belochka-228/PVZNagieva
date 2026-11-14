using Api_Nagieva.Data;
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
            return await _context.
        }
    }
}
