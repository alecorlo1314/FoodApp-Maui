using FoodApp.Domain.Common;
using MediatR;

namespace FoodApp.Aplicacion.Features.Cart.Commands.UpdateQuantity;

public record UpdateCartQuantityCommand(long CartItemId, int NewQuantity) : IRequest<Result<bool>>;
