using FoodApp.Aplicacion.Features.Location.Queries.GetLatLongLocation;

namespace FoodApp.Aplicacion.Common.Interfaces;

public interface ILocationService
{
    Task<string> GetCurrentAddressAsync(CancellationToken cancellationToken = default);
    Task<CoordenadaDto> GetLatLongAsync(CancellationToken cancellationToken = default);
}
