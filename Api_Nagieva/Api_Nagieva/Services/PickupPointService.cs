using Api_Nagieva.DTO;
using Api_Nagieva.Models;
using Microsoft.EntityFrameworkCore;

namespace Api_Nagieva.Services
{
    public class PickupPointService
    {
        private readonly MarketplaceDBContext _context;

        public PickupPointService(MarketplaceDBContext context)
        {
            _context = context;
        }

        public async Task<List<PickupPointResponse>> GetAllAsync()
        {
            return await _context.PickupPoints.Select(p => new PickupPointResponse(p.Id, p.Name, p.Address)).ToListAsync();
        }

        public async Task<PickupPointResponse> CreateAsync(CreatePickupPointRequest request)
        {
            var pp = new PickupPoint { Name = request.Name, Address = request.Address };
            _context.PickupPoints.Add(pp);
            await _context.SaveChangesAsync();
            return new PickupPointResponse(pp.Id, pp.Name, pp.Address);
        }
        public async Task<bool> UpdateAsync(int id, CreatePickupPointRequest request)
        {
            var pp = await _context.PickupPoints.FindAsync(id);
            if (pp == null) return false;

            pp.Name = request.Name;
            pp.Address = request.Address;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var pp = await _context.PickupPoints.FindAsync(id);
            if (pp == null) return false;

            _context.PickupPoints.Remove(pp);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
