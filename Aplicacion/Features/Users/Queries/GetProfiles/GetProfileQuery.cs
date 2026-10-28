using FoodApp.Domain.Common;
using FoodApp.Domain.Entities;
using MediatR;

namespace FoodApp.Aplicacion.Features.Users.Queries.GetProfiles;

public record GetProfileQuery() : IRequest<Result<Profile>>;
