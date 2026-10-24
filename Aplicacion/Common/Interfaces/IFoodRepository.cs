using FoodApp.Domain.Entities;

namespace FoodApp.Aplicacion.Common.Interfaces;

public interface IFoodRepository
{
    Task<IEnumerable<FoodItem>> GetFoodsByCategoryAsync(long restaurantId, long categoryId, CancellationToken cancellationToken = default);
}
