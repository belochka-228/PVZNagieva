namespace Api_Nagieva.DTO
{
    public record CartItemResponse(int ProductId, string ProductName, decimal Price, int Quantity, decimal TotalPrice, string? ImagePath );
    public record AddToCartRequest(int ProductId, int Quantity);
    public record UpdateCartQuantityRequest(int ProductId, int Quantity);
}
