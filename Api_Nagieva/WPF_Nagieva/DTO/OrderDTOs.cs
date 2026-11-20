using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Nagieva.DTO
{
    public class PickupPointResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
    }
    public class OrderItemRequest
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
    public class CreateOrderRequest
    {
        public string DeliveryType { get; set; } = string.Empty;
        public string DeliveryAddress { get; set; } = string.Empty;
        public int? PickupPointId { get; set; }
        public List<OrderItemRequest> Items { get; set; } = new List<OrderItemRequest>();
    }
    public class OrderResponse
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public string DeliveryType { get; set; } = string.Empty;
        public string DeliveryAddress { get; set; } = string.Empty;
        public string PickupPointAddress { get; set; } = string.Empty;
        public string UserLogin {  get; set; } = string.Empty;

        public List<OrderItemResponse> Items { get; set; } = new List<OrderItemResponse>();
    }

    public class OrderItemResponse
    {
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal PriceAtPurchase { get; set; }
    }
}