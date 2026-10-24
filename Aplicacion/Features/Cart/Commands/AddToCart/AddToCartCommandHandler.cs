using FoodApp.Aplicacion.Common.Interfaces;
using FoodApp.Domain.Common;
using MediatR;

namespace FoodApp.Aplicacion.Features.Cart.Commands.AddToCart;

public class AddToCartCommandHandler(ICartRepository _cartRepository, IAuthRepository _authService) : IRequestHandler<AddToCartCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(AddToCartCommand request, CancellationToken cancellationToken)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            var currentUser = _authService.GetCurrentUser();

            if (string.IsNullOrEmpty(currentUser.Value.Id.ToString()))
            {
                return Result.Failure<bool>("No se encontró una sesión activa.");
            }

            // Asignamos el ID del usuario autenticado actual al CartItem
            request.CartItem.UserId = currentUser.Value.Id;

            await _cartRepository.AddItemAsync(request.CartItem, cancellationToken);
            return Result.Success(true);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            return Result.Failure<bool>("La operación fue cancelada.");
        }
        catch (Exception ex)
        {
            return Result.Failure<bool>(ex.Message);
        }
    }
}
