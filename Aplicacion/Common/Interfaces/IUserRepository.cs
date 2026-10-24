namespace FoodApp.Aplicacion.Common.Interfaces;

public interface IUserRepository
{
    Task<string> GetUserNameAsync(CancellationToken cancellationToken = default);
}
