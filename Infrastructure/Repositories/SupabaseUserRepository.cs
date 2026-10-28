using FoodApp.Aplicacion.Common.Interfaces;
using FoodApp.Domain.Entities;
using FoodApp.Infrastructure.Models;
using Supabase.Gotrue;

namespace FoodApp.Infrastructure.Repositories;

public class SupabaseUserRepository(Supabase.Client _supabaseClient) : IUserRepository
{
    public async Task<Profile?> GetProfileByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        try
        {
            // Consulta nativa a la tabla profiles de Supabase filtrando por ID
            var response = await _supabaseClient
                .From<ProfileModel>()
                .Select("id,name,email,photo,bio,phone_number")
                .Where(x => x.Id == userId)
                .Single(cancellationToken);

            return response?.ToDomain();
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception)
        {
            return null;
        }
    }

    public async Task<string> GetUserNameAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        try
        {
            // Obtenemos el usuario autenticado actual desde el Auth de Supabase
            var currentUser = _supabaseClient.Auth.CurrentUser;

            if (currentUser == null) return "Invitado";

            // Consultamos su perfil en la tabla 'profiles' usando su ID
            var response = await _supabaseClient
                .From<ProfileModel>()
                .Where(p => p.Id == Guid.Parse(currentUser.Id))
                .Get(cancellationToken: cancellationToken);

            var profile = response.Models.FirstOrDefault();

            return profile?.Name ?? "Usuario";
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception)
        {
            return "Usuario";
        }
    }
}
