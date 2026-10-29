using FoodApp.Domain.Common;
using MediatR;

namespace FoodApp.Aplicacion.Features.Restaurants.Queries.GetRestaurants
{
    public record GetRestaurantsQuery() : IRequest<Result<IEnumerable<RestaurantDto>>>;
}