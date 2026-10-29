using FoodApp.Domain.Common;
using FoodApp.Domain.Entities;
using MediatR;

namespace FoodApp.Aplicacion.Features.Authentication.Commands.CurrentUser;

public record GetCurrentUser() : IRequest<Result<CurrentUserDto>>;
