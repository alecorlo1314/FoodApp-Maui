using FoodApp.Aplicacion.Common.Interfaces;
using FoodApp.Domain.Common;
using MediatR;

namespace FoodApp.Aplicacion.Features.Payments.Queries.GetPaymentMethods;

public class GetPaymentMethodsQueryHandler(IUserPaymentRepository _paymentRepository, IAuthRepository _authService) : 
    IRequestHandler<GetPaymentMethodsQuery, Result<IEnumerable<UserPaymentMethodDto>>>
{
    public async Task<Result<IEnumerable<UserPaymentMethodDto>>> Handle(GetPaymentMethodsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            var userId = _authService.GetCurrentUser();

            if (string.IsNullOrEmpty(userId.Value.Id.ToString()))
                return Result.Failure<IEnumerable<UserPaymentMethodDto>>("Usuario no autenticado.");

            var cards = await _paymentRepository.GetCardsByUserIdAsync(userId.Value.Id.ToString(), cancellationToken);
            return Result.Success(cards);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            return Result.Failure<IEnumerable<UserPaymentMethodDto>>("La operación fue cancelada.");
        }
        catch (Exception ex)
        {
            return Result.Failure<IEnumerable<UserPaymentMethodDto>>(ex.Message);
        }
    }
}