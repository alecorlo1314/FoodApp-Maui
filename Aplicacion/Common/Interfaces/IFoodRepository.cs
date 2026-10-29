using FoodApp.Aplicacion.Features.Foods.Queries.GetFoodsByCategory;

namespace FoodApp.Aplicacion.Common.Interfaces;

public interface IFoodRepository
{
    Task<IEnumerable<FoodItemDto>> GetFoodsByCategoryAsync(long restaurantId, long categoryId, CancellationToken cancellationToken = default);
}
