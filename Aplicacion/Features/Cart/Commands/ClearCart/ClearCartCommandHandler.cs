using FoodApp.Aplicacion.Common.Interfaces;
using FoodApp.Domain.Common;
using MediatR;

namespace FoodApp.Aplicacion.Features.Cart.Commands.ClearCart;

public class ClearCartCommandHandler(ICartRepository cart, IAuthRepository auth)
    : IRequestHandler<ClearCartCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(ClearCartCommand request, CancellationToken cancellationToken)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            var user = auth.GetCurrentUser();
            if (user.IsFailure || user.Value is null || user.Value.Id == Guid.Empty || user.Value.Id != request.UserId)
                return Result.Failure<bool>("La sesión no corresponde al usuario del carrito.");
            await cart.ClearAsync(request.UserId, cancellationToken);
            var remaining = await cart.GetCartItemsAsync(request.UserId.ToString(), cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            return remaining.Any()
                ? Result.Failure<bool>("No se pudo confirmar la limpieza del carrito.")
                : Result.Success(true);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex)
        {
            return Result.Failure<bool>($"No se pudo vaciar el carrito: {ex.Message}");
        }
    }
}
