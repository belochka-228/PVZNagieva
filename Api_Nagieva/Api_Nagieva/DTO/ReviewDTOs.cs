namespace Api_Nagieva.DTO
{
    public record ReviewResponse(int Id, int ProductId, string UserLogin, int Rating, string? Comment, DateTime CreatedAt);
    public record CreateReviewRequest( int ProductId, int Rating, string? Comment);
}
