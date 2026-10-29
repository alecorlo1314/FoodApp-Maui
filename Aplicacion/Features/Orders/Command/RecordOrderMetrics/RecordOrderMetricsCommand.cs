using FoodApp.Domain.Common;
using FoodApp.Domain.Entities;
using MediatR;

namespace FoodApp.Aplicacion.Features.Orders.Command.RecordOrderMetrics;

public record RecordOrderMetricsCommand(IReadOnlyList<OrderItemPayload> Items, Guid UserId, string PaymentId)
    : IRequest<Result<bool>>;
