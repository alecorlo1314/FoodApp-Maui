using FoodApp.Domain.Common;
using MediatR;

namespace FoodApp.Aplicacion.Features.Users.Queries.GetProfiles;

public record GetProfileQuery() : IRequest<Result<ProfileDto>>;
