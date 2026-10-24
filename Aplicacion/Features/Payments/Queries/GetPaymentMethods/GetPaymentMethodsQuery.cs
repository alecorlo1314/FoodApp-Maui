using FoodApp.Domain.Common;
using FoodApp.Domain.Entities;
using MediatR;

namespace FoodApp.Aplicacion.Features.Payments.Queries.GetPaymentMethods;

public record GetPaymentMethodsQuery() : IRequest<Result<IEnumerable<UserPaymentMethod>>>;
