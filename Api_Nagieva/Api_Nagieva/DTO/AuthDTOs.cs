namespace Api_Nagieva.DTO
{
    public record LoginRequest(string Login, string Password);
    public record RegisterRequest(string Login, string Password);
    public record AuthResponse(string AccessToken, string Role, string Login);
}
