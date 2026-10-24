using FoodApp.Domain.Common;
using FoodApp.Domain.Entities;
using MediatR;

namespace FoodApp.Aplicacion.Features.Restaurants.Queries.GetRestaurants
{
    public record GetRestaurantsQuery() : IRequest<Result<IEnumerable<Restaurant>>>;
}