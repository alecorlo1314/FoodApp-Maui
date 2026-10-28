using FoodApp.Domain.Entities;

namespace FoodApp.Aplicacion.Common.Interfaces;

public interface IUserRepository
{
    Task<string> GetUserNameAsync(CancellationToken cancellationToken = default);
    Task<Profile?> GetProfileByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
