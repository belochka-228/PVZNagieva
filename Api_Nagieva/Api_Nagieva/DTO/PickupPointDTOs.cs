namespace Api_Nagieva.DTO
{
    public record PickupPointResponse(int Id, string Name, string Address);
    public record CreatePickupPointRequest(string Name, string Address);
}
