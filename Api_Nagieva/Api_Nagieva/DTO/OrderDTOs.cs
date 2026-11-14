namespace Api_Nagieva.DTO
{
    public record CreateOrderRequest(string DeliveryType, string? DeliveryAddress, int? PickupPointId, List<OrderItemRequest>? Items);
    public record OrderItemRequest(int ProductId, int Quantity);
    public record OrderResponse(int Id, DateTime CreatedAt, decimal TotalAmount, string Status, string DeliveryType,
        string? DeliveryAddress, string? PickupPointAddress, List<OrderItemResponse> Items);
    public record OrderItemResponse( string ProductName, int Quantity, decimal PriceAtPurchase);
}
