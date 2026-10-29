using FoodApp.Aplicacion.Common.Interfaces;
using FoodApp.Aplicacion.Features.Restaurants.Queries.GetRestaurants;
using FoodApp.Infrastructure.Models;
using Supabase;

namespace FoodApp.Infrastructure.Repositories
{
    public class RestaurantRepository(Client _supabaseClient) : IRestaurantRepository
    {
        #region Obtener Restaurantes
        public async Task<IEnumerable<RestaurantDto>> GetRestaurantsAsync(
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var response = await _supabaseClient
                .From<RestaurantModel>()
                .Get(cancellationToken: cancellationToken);

            return response.Models.Select(m => m.ToDto());
        }

        #endregion
    }
}