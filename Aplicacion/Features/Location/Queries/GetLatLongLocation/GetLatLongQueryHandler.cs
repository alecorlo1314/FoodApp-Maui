using FoodApp.Aplicacion.Common.Interfaces;
using FoodApp.Domain.Common;
using MediatR;

namespace FoodApp.Aplicacion.Features.Location.Queries.GetLatLongLocation;

public class GetLatLongQueryHandler(ILocationService _locationService) : IRequestHandler<GetLatLongQuery, Result<CoordenadaDto>>
{
    public async Task<Result<CoordenadaDto>> Handle(GetLatLongQuery request, CancellationToken cancellationToken)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            var coordenadas = await _locationService.GetLatLongAsync(cancellationToken);
            return Result.Success(coordenadas);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            return Result.Failure<CoordenadaDto>("La operación fue cancelada.");
        }
        catch (Exception ex)
        {
            return Result.Failure<CoordenadaDto>(ex.Message);
        }
    }
}
