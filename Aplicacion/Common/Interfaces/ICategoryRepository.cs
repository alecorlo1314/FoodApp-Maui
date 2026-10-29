using FoodApp.Aplicacion.Features.Categories.Queries.GetCategories;

namespace FoodApp.Aplicacion.Common.Interfaces;

public interface ICategoryRepository
{
    Task<IEnumerable<CategoryDto>> GetCategoriesAsync(CancellationToken cancellationToken = default);
}