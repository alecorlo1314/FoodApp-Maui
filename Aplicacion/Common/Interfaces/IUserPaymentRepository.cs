using FoodApp.Aplicacion.Features.Payments.Queries.GetPaymentMethods;

namespace FoodApp.Aplicacion.Common.Interfaces;

public interface IUserPaymentRepository
{
    Task<IEnumerable<UserPaymentMethodDto>> GetCardsByUserIdAsync(string userId, CancellationToken cancellationToken = default);
    Task AddCardAsync(UserPaymentMethodDto card, CancellationToken cancellationToken = default);
}
