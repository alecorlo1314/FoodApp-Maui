using MediatR;
using FoodApp.Domain.Common;

namespace FoodApp.Aplicacion.Features.Authentication.Commands.Login;

public record LoginCommand(string Email, string Password) : IRequest<Result>;
