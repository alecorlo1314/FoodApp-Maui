using FoodApp.Aplicacion.Common.Interfaces;
using MediatR;
using FoodApp.Domain.Common;

namespace FoodApp.Aplicacion.Features.Authentication.Commands.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result>
{
    private readonly IAuthRepository _authRepository;

    public RegisterCommandHandler(IAuthRepository authRepository)
    {
        _authRepository = authRepository;
    }

    public async Task<Result> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            // Delegamos a infraestructura y retornamos el Result obtenido
            return await _authRepository.RegisterAsync(request.Name, request.Email, request.Password, cancellationToken);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            return Result.Failure("La operación fue cancelada.");
        }
    }
}
