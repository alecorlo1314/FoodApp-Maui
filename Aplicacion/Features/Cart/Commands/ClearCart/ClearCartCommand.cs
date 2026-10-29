using FoodApp.Domain.Common;
using MediatR;

namespace FoodApp.Aplicacion.Features.Cart.Commands.ClearCart;

public record ClearCartCommand(Guid UserId) : IRequest<Result<bool>>;
