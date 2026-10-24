using FoodApp.Domain.Entities;

namespace FoodApp.Aplicacion.Common.Interfaces;

public interface IUserPaymentRepository
{
    Task<IEnumerable<UserPaymentMethod>> GetCardsByUserIdAsync(string userId, CancellationToken cancellationToken = default);
    Task AddCardAsync(UserPaymentMethod card, CancellationToken cancellationToken = default);
}
