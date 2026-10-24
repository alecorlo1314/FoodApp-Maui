using FoodApp.Domain.Common;
using MediatR;

namespace FoodApp.Aplicacion.Features.Cart.Commands.RemoveCartItem;

public record RemoveCartItemCommand(long CartItemId) : IRequest<Result<bool>>;
