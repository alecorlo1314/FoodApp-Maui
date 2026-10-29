using FoodApp.Aplicacion.Features.Restaurants.Queries.GetRestaurants;

namespace FoodApp.Aplicacion.Common.Interfaces;

public interface IRestaurantRepository
{
    Task<IEnumerable<RestaurantDto>> GetRestaurantsAsync(CancellationToken cancellationToken = default);
}