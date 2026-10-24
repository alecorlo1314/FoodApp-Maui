using MediatR;

namespace FoodApp.Aplicacion.Features.Users.Queries.GetActiveUserProfile;

public record GetActiveUserProfileQuery() : IRequest<string>;
