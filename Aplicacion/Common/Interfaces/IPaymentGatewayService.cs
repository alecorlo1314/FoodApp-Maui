using FoodApp.Domain.Common;

namespace FoodApp.Aplicacion.Common.Interfaces;

public interface IPaymentGatewayService
{
    Task<Result<string>> ProcessPaymentAsync(decimal amount, string currency, string cardNumber, string expMonth, string expYear, string cvc, CancellationToken cancellationToken = default);
}
