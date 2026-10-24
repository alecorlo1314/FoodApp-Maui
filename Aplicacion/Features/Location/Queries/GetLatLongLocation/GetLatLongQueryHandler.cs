using FoodApp.Aplicacion.Common.Interfaces;
using FoodApp.Domain.Common;
using FoodApp.Domain.Entities;
using MediatR;

namespace FoodApp.Aplicacion.Features.Location.Queries.GetLatLongLocation;

public class GetLatLongQueryHandler(ILocationService _locationService) : IRequestHandler<GetLatLongQuery, Result<Coordenada>>
{
    public async Task<Result<Coordenada>> Handle(GetLatLongQuery request, CancellationToken cancellationToken)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            var coordenadas = await _locationService.GetLatLongAsync(cancellationToken);
            return Result.Success(coordenadas);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            return Result.Failure<Coordenada>("La operación fue cancelada.");
        }
        catch (Exception ex)
        {
            return Result.Failure<Coordenada>(ex.Message);
        }
    }
}
