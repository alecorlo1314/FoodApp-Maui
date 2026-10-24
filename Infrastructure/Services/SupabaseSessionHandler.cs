using Supabase.Gotrue;
using Supabase.Gotrue.Interfaces;
using System.Text.Json;

namespace FoodApp.Infrastructure.Services;

public class SupabaseSessionHandler : IGotrueSessionPersistence<Session>
{
    private const string SessionKey = "supabase_user_session";

    public void DestroySession()
    {
        try
        {
            SecureStorage.Remove(SessionKey);
        }
        catch (Exception)
        {
            // Manejo de errores de almacenamiento local si fuera necesario
        }
    }

    public Session LoadSession()
    {
        try
        {
            // Leemos la sesión guardada de forma segura en el dispositivo
            var sessionJson = SecureStorage.GetAsync(SessionKey).Result;
            if (!string.IsNullOrEmpty(sessionJson))
            {
                return JsonSerializer.Deserialize<Session>(sessionJson);
            }
        }
        catch (Exception)
        {
            // Si falla la deserialización o lectura
        }

        return null;
    }

    public void SaveSession(Session session)
    {
        try
        {
            // Guardamos la sesión serializada en el almacenamiento seguro
            var sessionJson = JsonSerializer.Serialize(session);
            SecureStorage.SetAsync(SessionKey, sessionJson).Wait();
        }
        catch (Exception)
        {
            // Manejo de errores al guardar
        }
    }
}