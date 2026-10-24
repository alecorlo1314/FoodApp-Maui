using FoodApp.Domain.Common;
using FoodApp.Domain.Entities;

namespace FoodApp.Aplicacion.Common.Interfaces;

public interface IAuthRepository
{
    Task<Result> RegisterAsync(string name, string email, string password, CancellationToken cancellationToken = default);
    Task<Result> LoginAsync(string email, string password, CancellationToken cancellationToken = default);
    Result<UsuarioActual> GetCurrentUser();
}