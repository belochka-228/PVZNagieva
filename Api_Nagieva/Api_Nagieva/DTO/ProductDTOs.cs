namespace Api_Nagieva.DTO
{
    public record ProductResponse( int Id, string Name, string? Description, decimal Price, string? ImagePath, int CategoryId,
        string CategoryName, int StockQuantity, int? PackQuantity, bool IsAvailable, double AverageRating);

    public record CreateProductRequest(string Name, string? Description, decimal Price, string? ImagePath, int CategoryId,
        int StockQuantity, int? PackQuantity, bool IsAvailable);
}
