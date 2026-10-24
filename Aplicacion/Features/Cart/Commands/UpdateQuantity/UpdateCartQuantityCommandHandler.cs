using FoodApp.Aplicacion.Common.Interfaces;
using FoodApp.Domain.Common;
using MediatR;

namespace FoodApp.Aplicacion.Features.Cart.Commands.UpdateQuantity;

public class UpdateCartQuantityCommandHandler(ICartRepository _cartRepository) 
    : IRequestHandler<UpdateCartQuantityCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(UpdateCartQuantityCommand request, CancellationToken cancellationToken)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            await _cartRepository.UpdateQuantityAsync(request.CartItemId, request.NewQuantity, cancellationToken);
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
