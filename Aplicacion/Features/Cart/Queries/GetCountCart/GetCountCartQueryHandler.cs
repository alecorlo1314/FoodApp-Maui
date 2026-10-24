using FoodApp.Aplicacion.Common.Interfaces;
using FoodApp.Domain.Common;
using MediatR;

namespace FoodApp.Aplicacion.Features.Cart.Queries.GetCountCart
{
    public class GetCountCartQueryHandler(ICartRepository _cartRepository, IAuthRepository authRepository) :
        IRequestHandler<GetCountCartQuery, Result<int>>
    {
        public async Task<Result<int>> Handle(GetCountCartQuery request, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                var usuarioActual = authRepository.GetCurrentUser();

                if (!usuarioActual.IsSuccess)
                {
                    return Result.Failure<int>(usuarioActual.ErrorMessage);
                }

                var items = await _cartRepository.GetCountCartQuery(usuarioActual.Value.Id.ToString(), cancellationToken);

                return Result.Success(items);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                return Result.Failure<int>("La operación fue cancelada.");
            }
            catch (Exception ex)
            {
                return Result.Failure<int>(ex.Message);
            }
        }
    }
}
