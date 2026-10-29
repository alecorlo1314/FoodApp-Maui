using FoodApp.Domain.Common;
using MediatR;

namespace FoodApp.Aplicacion.Features.Payments.Queries.GetPaymentMethods;

public record GetPaymentMethodsQuery() : IRequest<Result<IEnumerable<UserPaymentMethodDto>>>;
