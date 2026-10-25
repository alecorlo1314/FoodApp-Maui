using FoodApp.Domain.Entities;

namespace FoodApp.Aplicacion.Common.Interfaces;

public interface IProductMetricsRepository
{
    Task RecordAsync(Guid userId, string paymentId, IReadOnlyList<OrderItemPayload> items,
        CancellationToken cancellationToken = default);
}
