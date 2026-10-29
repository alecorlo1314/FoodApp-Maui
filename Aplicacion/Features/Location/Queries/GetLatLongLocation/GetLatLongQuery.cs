using FoodApp.Domain.Common;
using MediatR;

namespace FoodApp.Aplicacion.Features.Location.Queries.GetLatLongLocation;

public record GetLatLongQuery() : IRequest<Result<CoordenadaDto>>;
