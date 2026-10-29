using FoodApp.Domain.Common;
using FoodApp.Aplicacion.Features.Payments.Queries.GetPaymentMethods;
using MediatR;

namespace FoodApp.Aplicacion.Features.Payments.Commands.AddPaymenthMethod;

public record AddPaymentMethodCommand(UserPaymentMethodDto Card) : IRequest<Result<bool>>;
