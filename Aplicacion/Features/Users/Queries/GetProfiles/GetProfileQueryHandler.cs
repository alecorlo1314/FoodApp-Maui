using FoodApp.Domain.Common;
using FoodApp.Domain.Entities;
using FoodApp.Aplicacion.Common.Interfaces;
using MediatR;

namespace FoodApp.Aplicacion.Features.Users.Queries.GetProfiles;

public class GetProfileQueryHandler(IAuthRepository supabaseAuthRepository, IUserRepository _profileRepository) : IRequestHandler<GetProfileQuery, Result<Profile>>
{
    public async Task<Result<Profile>> Handle(GetProfileQuery request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        //1. Obtenenemos el usuario activo
        var currentUser = supabaseAuthRepository.GetCurrentUser();

        if (currentUser.IsFailure)
            return Result.Failure<Profile>(currentUser.ErrorMessage);

        // 2. Obtiene el modelo desde la capa de datos/repositorio
        var repoModel = await _profileRepository.GetProfileByIdAsync(currentUser.Value.Id, cancellationToken);

        if (repoModel == null)
            return Result.Failure<Profile>("No se pudo obtener el perfil.");

        return Result.Success(repoModel);
    }
}
