using FoodApp.Aplicacion.Common.Interfaces;
using FoodApp.Domain.Entities;

namespace FoodApp.Infrastructure.Services;

public class LocationService : ILocationService
{
    public async Task<string> GetCurrentAddressAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            // 1. Solicitar permisos de ubicación al sistema
            var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
            cancellationToken.ThrowIfCancellationRequested();

            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                cancellationToken.ThrowIfCancellationRequested();
                if (status != PermissionStatus.Granted)
                {
                    return "Permiso de ubicación denegado";
                }
            }

            // 2. Obtener la última ubicación conocida (latitud y longitud) con alta precisión
            var request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));
            var location = await Geolocation.GetLocationAsync(request, cancellationToken);

            if (location == null)
            {
                // Si falla el GPS, intentamos obtener la última cacheada
                location = await Geolocation.GetLastKnownLocationAsync();
                cancellationToken.ThrowIfCancellationRequested();
            }

            if (location != null)
            {
                // 3. Geocoding inverso: transformar Lat/Lng en texto de dirección legible de forma gratuita
                var placemarks = await Geocoding.GetPlacemarksAsync(location.Latitude, location.Longitude);
                cancellationToken.ThrowIfCancellationRequested();
                var placemark = placemarks?.FirstOrDefault();

                if (placemark != null)
                {
                    // Construimos una dirección limpia (Ej: Calle, Ciudad)
                    string address = $"{placemark.Thoroughfare ?? ""} {placemark.SubThoroughfare ?? ""}, {placemark.Locality ?? placemark.SubAdminArea ?? ""}".Trim();

                    return string.IsNullOrWhiteSpace(address) ? "Ubicación actual detectada" : address;
                }
            }

            return "No se pudo determinar la dirección";
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex)
        {
            return $"Error de GPS: {ex.Message}";
        }
    }


    public async Task<Coordenada> GetLatLongAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            var status = await Permissions
                .CheckStatusAsync<Permissions.LocationWhenInUse>();
            cancellationToken.ThrowIfCancellationRequested();

            if (status != PermissionStatus.Granted)
            {
                status = await Permissions
                    .RequestAsync<Permissions.LocationWhenInUse>();
                cancellationToken.ThrowIfCancellationRequested();

                if (status != PermissionStatus.Granted)
                    return new Coordenada();
            }

            if (!Geolocation.Default.IsEnabled)
                return new Coordenada();

            // Primero intentar obtener ubicación cacheada
            var location = await Geolocation.Default
                .GetLastKnownLocationAsync();
            cancellationToken.ThrowIfCancellationRequested();

            // Si no existe, solicitar una ubicación actual
            if (location is null)
            {
                using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                cts.CancelAfter(TimeSpan.FromSeconds(10));

                var request = new GeolocationRequest(
                    GeolocationAccuracy.Medium,
                    TimeSpan.FromSeconds(10));

                location = await Geolocation.Default
                    .GetLocationAsync(request, cts.Token);
            }

            if (location is null)
                return new Coordenada();

            return new Coordenada
            {
                Latitude = location.Latitude,
                Longitude = location.Longitude
            };
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(
                $"Error de ubicación: {ex.Message}");

            return new Coordenada();
        }
    }


}
