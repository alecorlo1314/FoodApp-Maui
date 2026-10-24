using FoodApp.Domain.Common;
using FoodApp.Domain.Entities;
using MediatR;

namespace FoodApp.Aplicacion.Features.Location.Queries.GetLatLongLocation;

public record GetLatLongQuery() : IRequest<Result<Coordenada>>;
