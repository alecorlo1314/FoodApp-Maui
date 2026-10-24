using FoodApp.Domain.Entities;

namespace FoodApp.Aplicacion.Common.Interfaces;

public interface IRestaurantRepository
{
    Task<IEnumerable<Restaurant>> GetRestaurantsAsync(CancellationToken cancellationToken = default);
}