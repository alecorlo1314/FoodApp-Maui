using FoodApp.Aplicacion.Common.Interfaces;
using FoodApp.Domain.Entities;
using FoodApp.Infrastructure.Models;
using Supabase;
using Supabase.Postgrest;

namespace FoodApp.Infrastructure.Repositories;

public class ProductMetricsRepository(Supabase.Client client) : IProductMetricsRepository
{
    public async Task RecordAsync(Guid userId, string paymentId, IReadOnlyList<OrderItemPayload> items,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var models = items.Select(item => new ProductMetricModel
        {
            UserId = userId,
            PaymentId = paymentId,
            ProductId = item.ProductId,
            QuantityOrdered = item.Quantity
        }).ToList();

        // Una escritura atómica; el modelo usa la clave de negocio y omite id.
        // Upsert activa resolution=ignore-duplicates para reintentar el mismo pago.
        await client.From<ProductMetricModel>().Insert(models, new QueryOptions
        {
            Upsert = true,
            OnConflict = "user_id,payment_id,product_id",
            DuplicateResolution = QueryOptions.DuplicateResolutionType.IgnoreDuplicates,
            Returning = QueryOptions.ReturnType.Minimal
        }, cancellationToken);

        var saved = await client.From<ProductMetricModel>()
            .Where(metric => metric.UserId == userId && metric.PaymentId == paymentId)
            .Get(cancellationToken: cancellationToken);
        if (saved.Models.Count != models.Count || models.Any(expected =>
            !saved.Models.Any(actual => actual.ProductId == expected.ProductId &&
                actual.QuantityOrdered == expected.QuantityOrdered)))
            throw new InvalidOperationException("No se confirmó el registro completo de las métricas.");
    }
}
