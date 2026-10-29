using FoodApp.Aplicacion.Common.Interfaces;
using FoodApp.Domain.Common;
using FoodApp.Domain.Entities;
using MediatR;

namespace FoodApp.Aplicacion.Features.Authentication.Commands.CurrentUser;

internal class GetCurrentUserQueryHandler(IAuthRepository authRepository)
    : IRequestHandler<GetCurrentUser, Result<CurrentUserDto>>
{
    public async Task<Result<CurrentUserDto>> Handle(GetCurrentUser request, CancellationToken cancellationToken)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            var usuarioActual =  authRepository.GetCurrentUser();
            return usuarioActual;
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            return Result.Failure<CurrentUserDto>("La operación fue cancelada.");
        }
        catch (Exception ex)
        {
            return Result.Failure<CurrentUserDto>(ex.Message);
        }
    }
}
