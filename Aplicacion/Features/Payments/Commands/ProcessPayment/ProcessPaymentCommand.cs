using FoodApp.Domain.Common;
using MediatR;

namespace FoodApp.Aplicacion.Features.Payments.Commands.ProcessPayment;

public record ProcessPaymentCommand(
        decimal Amount,
        string CardNumber,
        string ExpireDate,
        string Cvc,
        Guid CheckoutId = default
    ) : IRequest<Result<string>>;
