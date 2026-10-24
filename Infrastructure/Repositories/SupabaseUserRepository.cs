using FoodApp.Aplicacion.Common.Interfaces;
using FoodApp.Infrastructure.Models;

namespace FoodApp.Infrastructure.Repositories;

public class SupabaseUserRepository(Supabase.Client _supabaseClient) : IUserRepository
{
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
                .Where(p => p.Id == currentUser.Id)
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
