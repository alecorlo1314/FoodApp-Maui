using FoodApp.Aplicacion.Common.Interfaces;
using MediatR;
using FoodApp.Domain.Common;

namespace FoodApp.Aplicacion.Features.Authentication.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, Result>
{
    private readonly IAuthRepository _authRepository;

    public LoginCommandHandler(IAuthRepository authRepository)
    {
        _authRepository = authRepository;
    }

    public async Task<Result> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await _authRepository.LoginAsync(request.Email, request.Password, cancellationToken);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            return Result.Failure("La operación fue cancelada.");
        }
    }
}
