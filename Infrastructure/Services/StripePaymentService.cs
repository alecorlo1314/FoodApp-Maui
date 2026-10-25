using FoodApp.Aplicacion.Common.Interfaces;
using FoodApp.Domain.Common;
using System.Diagnostics;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace FoodApp.Infrastructure.Services;

public class StripePaymentService : IPaymentGatewayService
{
    private readonly HttpClient _httpClient;

    public StripePaymentService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<Result<string>> ProcessPaymentAsync(
        decimal amount,
        string currency,
        string cardNumber,
        string expMonth,
        string expYear,
        string cvc,
        CancellationToken cancellationToken = default,
        string? idempotencyKey = null)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (amount <= 0)
                return Result.Failure<string>("El monto debe ser mayor que cero.");

            // La función de prueba usa un PaymentMethod de Stripe; no recibe PAN ni CVC.
            var payload = new
            {
                amount = amount,
                currency = currency.ToLowerInvariant(),
                idempotency_key = idempotencyKey ?? Guid.NewGuid().ToString("D")
            };

            using var response = await _httpClient.PostAsJsonAsync(string.Empty, payload, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                return Result.Failure<string>($"Error en el servidor de pagos: {errorContent}");
            }

            // Deserializamos la respuesta de la Edge Function
            var resultData = await response.Content.ReadFromJsonAsync<EdgeFunctionPaymentResponse>(cancellationToken: cancellationToken);

            if (resultData is { Success: true } && !string.IsNullOrWhiteSpace(resultData.TransactionId))
            {
                return Result.Success(resultData.TransactionId);
            }

            return Result.Failure<string>(resultData?.Message ?? "El pago no pudo ser procesado.");
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error al invocar la Edge Function de pago: {ex.Message}");
            return Result.Failure<string>("Ocurrió un error inesperado al procesar el pago.");
        }
    }
}

// Modelo auxiliar para mapear la respuesta de tu Edge Function
internal class EdgeFunctionPaymentResponse
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("transactionId")]
    public string TransactionId { get; set; } = string.Empty;

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
}