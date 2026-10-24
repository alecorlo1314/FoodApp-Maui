using FoodApp.Domain.Common;
using FoodApp.Domain.Entities;
using MediatR;

namespace FoodApp.Aplicacion.Features.Payments.Commands.AddPaymenthMethod;

public record AddPaymentMethodCommand(UserPaymentMethod Card) : IRequest<Result<bool>>;
