using FoodApp.Aplicacion.Common.Interfaces;
using FoodApp.Domain.Common;
using MediatR;

namespace FoodApp.Aplicacion.Features.Payments.Commands.AddPaymenthMethod
{
    public class AddPaymentMethodCommandHandler(IUserPaymentRepository _paymentRepository, IAuthRepository _authService) :
        IRequestHandler<AddPaymentMethodCommand, Result<bool>>
    {
        public async Task<Result<bool>> Handle(AddPaymentMethodCommand request, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                var userId = _authService.GetCurrentUser();
                if (string.IsNullOrEmpty(userId.Value.Id.ToString()))
                    return Result.Failure<bool>("Usuario no autenticado.");

                request.Card.UserId = userId.Value.Id.ToString();
                await _paymentRepository.AddCardAsync(request.Card, cancellationToken);

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
}
