using FoodApp.Aplicacion.Features.Users.Queries.GetProfiles;

namespace FoodApp.Aplicacion.Common.Interfaces;

public interface IUserRepository
{
    Task<string> GetUserNameAsync(CancellationToken cancellationToken = default);
    Task<ProfileDto?> GetProfileByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
