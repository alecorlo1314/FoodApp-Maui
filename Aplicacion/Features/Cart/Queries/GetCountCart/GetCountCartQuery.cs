using FoodApp.Domain.Common;
using MediatR;

namespace FoodApp.Aplicacion.Features.Cart.Queries.GetCountCart;
public record GetCountCartQuery() : IRequest<Result<int>>;
