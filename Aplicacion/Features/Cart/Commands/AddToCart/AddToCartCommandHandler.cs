using FoodApp.Aplicacion.Common.Interfaces;
using FoodApp.Domain.Common;
using FoodApp.Domain.Entities;
using MediatR;

namespace FoodApp.Aplicacion.Features.Cart.Commands.AddToCart;

public class AddToCartCommandHandler(ICartRepository _cartRepository, IAuthRepository _authService) :
    IRequestHandler<AddToCartCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(AddToCartCommand request, CancellationToken cancellationToken)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            //Obtenemos el usuario actual
            var currentUser = _authService.GetCurrentUser();

            //Si no existe sesión activa devolvemos un error
            if (currentUser.IsFailure || currentUser.Value is null || currentUser.Value.Id == Guid.Empty)
            {
                return Result.Failure<bool>("No se encontró una sesión activa.");
            }

            // Asignamos el ID del usuario autenticado actual al CartItem
            var userId = currentUser.Value.Id;

            var cartItemDomain = new CartItemEntidad(
                            userId: userId,
                            foodId: request.CartItem.FoodId,
                            name: request.CartItem.Name,
                            imageUrl: request.CartItem.ImageUrl,
                            sizeName: request.CartItem.SizeName,
                            price: request.CartItem.Price,
                            quantity: request.CartItem.Quantity
                        );

            await _cartRepository.AddItemAsync(cartItemDomain, cancellationToken);
            return Result.Success(true);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            return Result.Failure<bool>("La operación fue cancelada.");
        }
        catch (ArgumentException ex)
        {
            // Captura errores de validación de los parámetros del constructor (ej. precio negativo, nombre vacío)
            return Result.Failure<bool>($"Error de validación: {ex.Message}");
        }
        catch (Exception ex)
        {
            return Result.Failure<bool>(ex.Message);
        }
    }
}
