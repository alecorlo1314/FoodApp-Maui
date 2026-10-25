using FoodApp.Aplicacion.Common.Interfaces;
using FoodApp.Domain.Common;
using FoodApp.Domain.Entities;
using MediatR;

namespace FoodApp.Aplicacion.Features.Orders.Command.RecordOrderMetrics;

public class RecordOrderMetricsCommandHandler(IProductMetricsRepository metrics, IAuthRepository auth)
    : IRequestHandler<RecordOrderMetricsCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(RecordOrderMetricsCommand request, CancellationToken cancellationToken)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            var user = auth.GetCurrentUser();
            if (user.IsFailure || user.Value is null || user.Value.Id == Guid.Empty || user.Value.Id != request.UserId)
                return Result.Failure<bool>("La sesión no corresponde al usuario del pago.");
            if (string.IsNullOrWhiteSpace(request.PaymentId) || request.Items is null || request.Items.Count == 0 ||
                request.Items.Any(item => item is null || item.ProductId <= 0 || item.Quantity <= 0))
                return Result.Failure<bool>("El pago y los productos de las métricas no son válidos.");

            var items = request.Items.GroupBy(item => item.ProductId)
                .Select(group => new OrderItemPayload
                {
                    ProductId = group.Key,
                    Quantity = group.Sum(item => item.Quantity)
                }).ToList();
            await metrics.RecordAsync(request.UserId, request.PaymentId, items, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            return Result.Success(true);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex)
        {
            return Result.Failure<bool>($"No se pudieron registrar las métricas: {ex.Message}");
        }
    }
}
