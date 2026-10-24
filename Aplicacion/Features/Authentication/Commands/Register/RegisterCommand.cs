using MediatR;
using FoodApp.Domain.Common;

namespace FoodApp.Aplicacion.Features.Authentication.Commands.Register;

public record RegisterCommand(string Name, string Email, string Password) : IRequest<Result>;
