using FoodApp.Domain.Entities;

namespace FoodApp.Aplicacion.Common.Interfaces;

public interface ILocationService
{
    Task<string> GetCurrentAddressAsync(CancellationToken cancellationToken = default);
    Task<Coordenada> GetLatLongAsync(CancellationToken cancellationToken = default);
}
