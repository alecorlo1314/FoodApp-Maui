using FoodApp.Aplicacion.Common.Interfaces;
using FoodApp.Domain.Common;
using FoodApp.Domain.Entities;
using MediatR;

namespace FoodApp.Aplicacion.Features.Payments.Queries.GetPaymentMethods;

public class GetPaymentMethodsQueryHandler(IUserPaymentRepository _paymentRepository, IAuthRepository _authService) : 
    IRequestHandler<GetPaymentMethodsQuery, Result<IEnumerable<UserPaymentMethod>>>
{
    public async Task<Result<IEnumerable<UserPaymentMethod>>> Handle(GetPaymentMethodsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            var userId = _authService.GetCurrentUser();

            if (string.IsNullOrEmpty(userId.Value.Id.ToString()))
                return Result.Failure<IEnumerable<UserPaymentMethod>>("Usuario no autenticado.");

            var cards = await _paymentRepository.GetCardsByUserIdAsync(userId.Value.Id.ToString(), cancellationToken);
            return Result.Success(cards);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            return Result.Failure<IEnumerable<UserPaymentMethod>>("La operación fue cancelada.");
        }
        catch (Exception ex)
        {
            return Result.Failure<IEnumerable<UserPaymentMethod>>(ex.Message);
        }
    }
}