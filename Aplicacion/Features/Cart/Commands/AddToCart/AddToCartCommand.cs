using FoodApp.Domain.Common;
using FoodApp.Domain.Entities;
using MediatR;

namespace FoodApp.Aplicacion.Features.Cart.Commands.AddToCart;

public record AddToCartCommand(CartItem CartItem) : IRequest<Result<bool>>;
