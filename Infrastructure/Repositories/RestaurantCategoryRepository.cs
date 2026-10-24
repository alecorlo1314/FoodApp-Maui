using FoodApp.Aplicacion.Common.Interfaces;
using FoodApp.Domain.Entities;
using FoodApp.Infrastructure.Models;
using Supabase;

namespace FoodApp.Infrastructure.Repositories
{
    public class RestaurantCategoryRepository(Client _supabaseClient) : IRestaurantCategoryRepository
    {
        public async Task<IEnumerable<RestaurantCategory>> GetCategoriesByRestaurantAsync(long restaurantId, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var response = await _supabaseClient
                .From<RestaurantCategoryModel>()
                .Where(x => x.RestaurantId == restaurantId)
                .Get(cancellationToken: cancellationToken);

            return response.Models.Select(m => m.ToDomain());
        }
    }
}