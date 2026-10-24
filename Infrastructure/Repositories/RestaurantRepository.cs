using FoodApp.Aplicacion.Common.Interfaces;
using FoodApp.Domain.Entities;
using FoodApp.Infrastructure.Models;
using Supabase;

namespace FoodApp.Infrastructure.Repositories
{
    public class RestaurantRepository(Client _supabaseClient) : IRestaurantRepository
    {
        public async Task<IEnumerable<Restaurant>> GetRestaurantsAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var response = await _supabaseClient
                .From<RestaurantModel>()
                .Get(cancellationToken: cancellationToken);

            return response.Models.Select(m => m.ToDomain());
        }
    }
}