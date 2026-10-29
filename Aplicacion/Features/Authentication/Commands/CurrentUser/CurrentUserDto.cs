namespace FoodApp.Aplicacion.Features.Authentication.Commands.CurrentUser;

public sealed record CurrentUserDto(
    Guid Id,
    string? Email);
