using FoodApp.Domain.Entities;

namespace FoodApp.Aplicacion.Common.Interfaces;

public interface IRestaurantCategoryRepository
{
    Task<IEnumerable<RestaurantCategory>> GetCategoriesByRestaurantAsync(long restaurantId, CancellationToken cancellationToken = default);
}