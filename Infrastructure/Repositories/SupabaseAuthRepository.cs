using FoodApp.Aplicacion.Common.Interfaces;
using FoodApp.Infrastructure.Models;
using FoodApp.Domain.Common;
using Supabase;
using FoodApp.Aplicacion.Features.Authentication.Commands.CurrentUser;


namespace FoodApp.Infrastructure.Repositories;

public class SupabaseAuthRepository(Client _supabaseClient) : IAuthRepository
{
    public Result<CurrentUserDto> GetCurrentUser()
    {
        try
        {
            var usuarioActual = _supabaseClient.Auth.CurrentUser;

            var session = _supabaseClient.Auth.CurrentSession;
            var user = _supabaseClient.Auth.CurrentUser;


            if (usuarioActual is null)
            {
                return Result.Failure<CurrentUserDto>(
                    "No hay un usuario autenticado.");
            }

            if (!Guid.TryParse(usuarioActual.Id, out var userId))
            {
                return Result.Failure<CurrentUserDto>(
                    "El identificador del usuario no es válido.");
            }

            var usuario = new CurrentUserDto(
                userId,
                usuarioActual.Email);

            return Result.Success(usuario);
        }
        catch (Exception)
        {
            return Result.Failure<CurrentUserDto>(
                "Ocurrió un error al obtener el usuario actual.");
        }
    }



    public async Task<Result> LoginAsync(
        string email,
        string password, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        try
        {
            // This SDK overload cannot abort Auth HTTP; check cancellation around it.
            var session = await _supabaseClient.Auth.SignIn(
                email, password);
            cancellationToken.ThrowIfCancellationRequested();

            if (session?.User is null)
            {
                return Result.Failure(
                    "No se pudo iniciar sesión.");
            }

            return Result.Success();
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex)
        {
            return Result.Failure(ex.Message);
        }
    }


    public async Task<Result> RegisterAsync(string name, string email, string password, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        try
        {
            // 1. Intentamos registrar al usuario en Supabase Auth
            // SignUp has no CancellationToken overload in the installed SDK.
            var session = await _supabaseClient.Auth.SignUp(email, password);
            cancellationToken.ThrowIfCancellationRequested();

            if (session?.User != null)
            {
                // 2. Si todo sale bien en Auth, guardamos el perfil adicional en la base de datos
                var newProfile = new ProfileModel
                {
                    Id = Guid.Parse(session.User.Id),
                    Name = name,
                };

                await _supabaseClient.From<ProfileModel>().Insert(newProfile, cancellationToken: cancellationToken);

                return Result.Success();
            }

            return Result.Failure("No se pudo completar el registro.");
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex)
        {
            // Detectamos si el error viene porque el correo ya está registrado en Supabase
            if (ex.Message.Contains("already registered", StringComparison.OrdinalIgnoreCase) ||
                ex.Message.Contains("already exists", StringComparison.OrdinalIgnoreCase))
            {
                return Result.Failure("El correo electrónico ya se encuentra registrado.");
            }

            return Result.Failure($"Ocurrió un error inesperado: {ex.Message}");
        }
    }
}
