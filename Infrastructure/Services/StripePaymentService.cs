using FoodApp.Aplicacion.Common.Interfaces;
using FoodApp.Domain.Common;
using System.Diagnostics;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace FoodApp.Infrastructure.Services;

public class StripePaymentService : IPaymentGatewayService
{
    private readonly HttpClient _httpClient;
    // O puedes inyectar tu cliente de Supabase configurado y obtener la URL y el anon/auth token

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
        CancellationToken cancellationToken = default)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (amount <= 0)
                return Result.Failure<string>("El monto debe ser mayor que cero.");

            // Estructura de los datos que espera tu Supabase Edge Function
            //var payload = new
            //{
            //    amount = amount,
            //    currency = currency.ToLowerInvariant(),
            //    card_number = cardNumber.Replace(" ", ""),
            //    exp_month = expMonth,
            //    exp_year = expYear,
            //    cvc = cvc
            //};
            var payload = new
            {
                amount = amount,
                currency = currency.ToLowerInvariant()
            };

            // URL de tu Supabase Edge Function (puedes configurarla en appsettings o constantes)
            //string functionUrl = "https://hdtsshzstljffllzoqkb.supabase.co/functions/v1/process-payment";
            string functionUrl = _httpClient.BaseAddress.ToString();

            // Realizamos la petición POST a la Edge Function
            var response = await _httpClient.PostAsJsonAsync(functionUrl, payload, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                return Result.Failure<string>($"Error en el servidor de pagos: {errorContent}");
            }

            // Deserializamos la respuesta de la Edge Function
            var resultData = await response.Content.ReadFromJsonAsync<EdgeFunctionPaymentResponse>(cancellationToken: cancellationToken);

            if (resultData != null && resultData.Success)
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