using FoodApp.Aplicacion.Common.Interfaces;
using FoodApp.Domain.Common;
using MediatR;

namespace FoodApp.Aplicacion.Features.Payments.Commands.ProcessPayment;

public class ProcessPaymentCommandHandler(IPaymentGatewayService _paymentGatewayService)
    : IRequestHandler<ProcessPaymentCommand, Result<string>>
{
    public async Task<Result<string>> Handle(ProcessPaymentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            // Separar MM/YY o MM/YYYY
            var parts = request.ExpireDate.Split('/');
            if (parts.Length != 2)
            {
                return Result.Failure<string>("La fecha de expiración debe tener el formato MM/YY");
            }

            string expMonth = parts[0];
            string expYear = parts[1];

            return await _paymentGatewayService.ProcessPaymentAsync(
                request.Amount,
                "usd",
                request.CardNumber,
                expMonth,
                expYear,
                request.Cvc,
                cancellationToken,
                (request.CheckoutId == Guid.Empty ? Guid.NewGuid() : request.CheckoutId).ToString("D"));
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            return Result.Failure<string>("La operación fue cancelada.");
        }
    }
}
