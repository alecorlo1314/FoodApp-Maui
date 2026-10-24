using FoodApp.Aplicacion.Common.Interfaces;
using FoodApp.Domain.Common;
using FoodApp.Domain.Entities;
using MediatR;

namespace FoodApp.Aplicacion.Features.Cart.Queries.GetCart;

public class GetCartQueryHandler(ICartRepository _cartRepository) : IRequestHandler<GetCartQuery, Result<IEnumerable<CartItem>>>
{
    public async Task<Result<IEnumerable<CartItem>>> Handle(GetCartQuery request, CancellationToken cancellationToken)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            var items = await _cartRepository.GetCartItemsAsync(request.UserId, cancellationToken);
            return Result.Success(items);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            return Result.Failure<IEnumerable<CartItem>>("La operación fue cancelada.");
        }
        catch (Exception ex)
        {
            return Result.Failure<IEnumerable<CartItem>>(ex.Message);
        }
    }
}
