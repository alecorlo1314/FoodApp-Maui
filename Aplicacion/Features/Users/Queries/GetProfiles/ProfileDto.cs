namespace FoodApp.Aplicacion.Features.Users.Queries.GetProfiles;

public sealed record ProfileDto(
    Guid Id,
    string? Name,
    string? Email,
    string? Photo,
    string? Bio,
    string? PhoneNumber);
