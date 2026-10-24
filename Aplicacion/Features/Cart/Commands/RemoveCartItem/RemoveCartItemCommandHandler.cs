using FoodApp.Aplicacion.Common.Interfaces;
using FoodApp.Domain.Common;
using MediatR;

namespace FoodApp.Aplicacion.Features.Cart.Commands.RemoveCartItem;

public class RemoveCartItemCommandHandler(ICartRepository _cartRepository) 
    : IRequestHandler<RemoveCartItemCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(RemoveCartItemCommand request, CancellationToken cancellationToken)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            await _cartRepository.RemoveItemAsync(request.CartItemId, cancellationToken);
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
