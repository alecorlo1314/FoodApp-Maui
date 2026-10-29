using FoodApp.Aplicacion.Features.Authentication.Commands.CurrentUser;
using FoodApp.Domain.Common;

namespace FoodApp.Aplicacion.Common.Interfaces;

public interface IAuthRepository
{
    Task<Result> RegisterAsync(string name, string email, string password, CancellationToken cancellationToken = default);
    Task<Result> LoginAsync(string email, string password, CancellationToken cancellationToken = default);
    Result<CurrentUserDto> GetCurrentUser();
}