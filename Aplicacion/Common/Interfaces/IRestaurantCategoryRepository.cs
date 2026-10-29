using FoodApp.Aplicacion.Features.RestaurantCategories.Queries.GetRestaurantCategories;

namespace FoodApp.Aplicacion.Common.Interfaces;

public interface IRestaurantCategoryRepository
{
    Task<IEnumerable<RestaurantCategoryDto>> GetCategoriesByRestaurantAsync(long restaurantId, CancellationToken cancellationToken = default);
}