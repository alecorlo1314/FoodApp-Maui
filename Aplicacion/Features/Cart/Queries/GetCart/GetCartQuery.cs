using FoodApp.Domain.Common;
using FoodApp.Domain.Entities;
using MediatR;

namespace FoodApp.Aplicacion.Features.Cart.Queries.GetCart;

public record GetCartQuery(string UserId) : IRequest<Result<IEnumerable<CartItem>>>;
