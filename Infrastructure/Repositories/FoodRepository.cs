using FoodApp.Aplicacion.Features.Foods.Queries.GetFoodsByCategory;
using FoodApp.Aplicacion.Common.Interfaces;
using FoodApp.Infrastructure.Models;
using Supabase;

namespace FoodApp.Infrastructure.Repositories;

public class FoodRepository(Client _supabaseClient) : IFoodRepository
{
    public async Task<IEnumerable<FoodItemDto>> GetFoodsByCategoryAsync(long restaurantId, long categoryId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var response = await _supabaseClient
            .From<FoodItemModel>()
            .Where(x => x.RestaurantId == restaurantId && x.RestaurantCategoryId == categoryId)
            .Get(cancellationToken: cancellationToken);

        return response.Models.Select(m => m.ToDto());
    }
}