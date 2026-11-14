using Api_Nagieva.DTO;
using Api_Nagieva.Models;
using Microsoft.EntityFrameworkCore;

namespace Api_Nagieva.Services
{
    public class OrderService
    {
        private readonly MarketplaceDBContext _context;

        public OrderService(MarketplaceDBContext context)
        {
            _context = context;
        }

        public async Task<OrderResponse> CreateOrderAsync(int userId, CreateOrderRequest request)
        {
            List<OrderItemRequest> itemsToBuy;

            if (request.Items != null && request.Items.Any())
            {
                itemsToBuy = request.Items;
            }
            else
            {
                var cart = await _context.CartItems.Where(c => c.UserId == userId).ToListAsync();
                if (!cart.Any()) throw new Exception("Корзина пуста");
                itemsToBuy = cart.Select(c => new OrderItemRequest(c.ProductId, c.Quantity)).ToList();
            }

            decimal totalAmount = 0;
            var orderItemsToCreate = new List<OrderItem>();

            foreach (var item in itemsToBuy)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product == null || !product.IsAvailable)
                    throw new Exception($"Товар {item.ProductId} недоступен");

                if (product.StockQuantity < item.Quantity)
                    throw new Exception($"Товара '{product.Name}' не хватает на складе (в наличии: {product.StockQuantity})");

                totalAmount += product.Price * item.Quantity;

                orderItemsToCreate.Add(new OrderItem
                {
                    ProductId = product.Id,
                    Quantity = item.Quantity,
                    PriceAtPurchase = product.Price
                });
            }

            if (totalAmount < 600 && request.DeliveryType == "Courier")
                throw new Exception("Доставка курьером доступна только для заказов от 600 рублей. Выберите ПВЗ.");

            var status = await _context.OrderStatuses.FirstOrDefaultAsync(s => s.Name == "Оформлен");
            if (status == null) throw new Exception("Статус 'Оформлен' не найден в БД");

            var order = new Order
            {
                UserId = userId,
                CreatedAt = DateTime.Now,
                StatusId = status.Id,
                DeliveryType = request.DeliveryType,
                DeliveryAddress = request.DeliveryAddress,
                PickupPointId = request.PickupPointId,
                TotalAmount = totalAmount,
                OrderItems = orderItemsToCreate
            };

            _context.Orders.Add(order);

            foreach (var item in itemsToBuy)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                product!.StockQuantity -= item.Quantity;
            }

            if (request.Items == null || !request.Items.Any())
            {
                var cart = await _context.CartItems.Where(c => c.UserId == userId).ToListAsync();
                _context.CartItems.RemoveRange(cart);
            }

            await _context.SaveChangesAsync();

            return await GetOrderByIdAsync(userId, order.Id) ?? throw new Exception("Ошибка при создании заказа");
        }

        public async Task<List<OrderResponse>> GetUserOrdersAsync(int userId, bool newestFirst = true)
        {
            var query = _context.Orders.Include(o => o.Status).Include(o => o.PickupPoint).Include(o => o.OrderItems).ThenInclude(oi => oi.Product)
                .Where(o => o.UserId == userId);
            query = newestFirst ? query.OrderByDescending(o => o.CreatedAt) : query.OrderBy(o => o.CreatedAt);

            return await query.Select(o => new OrderResponse(o.Id, o.CreatedAt, o.TotalAmount, o.Status.Name, o.DeliveryType,
                    o.DeliveryAddress, o.PickupPoint != null ? o.PickupPoint.Address : null,
                    o.OrderItems.Select(oi => new OrderItemResponse(oi.Product.Name, oi.Quantity, oi.PriceAtPurchase)).ToList())).ToListAsync();
        }

        public async Task<OrderResponse?> GetOrderByIdAsync(int userId, int orderId)
        {
            return await _context.Orders.Include(o => o.Status).Include(o => o.PickupPoint).Include(o => o.OrderItems).ThenInclude(oi => oi.Product)
                .Where(o => o.Id == orderId && o.UserId == userId)
                .Select(o => new OrderResponse(o.Id, o.CreatedAt, o.TotalAmount, o.Status.Name, o.DeliveryType,
                    o.DeliveryAddress, o.PickupPoint != null ? o.PickupPoint.Address : null,
                    o.OrderItems.Select(oi => new OrderItemResponse(oi.Product.Name, oi.Quantity, oi.PriceAtPurchase)).ToList())).FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateStatusAsync(int orderId, string newStatusName)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null) return false;

            var status = await _context.OrderStatuses.FirstOrDefaultAsync(s => s.Name == newStatusName);
            if (status == null) return false;

            order.StatusId = status.Id;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
