using FoodApp.Aplicacion.Common.Interfaces;
using FoodApp.Domain.Common;
using MediatR;

namespace FoodApp.Aplicacion.Features.Restaurants.Queries.GetRestaurants
{
    public class GetRestaurantsQueryHandler(IRestaurantRepository _restaurantRepository) : 
        IRequestHandler<GetRestaurantsQuery, Result<IEnumerable<RestaurantDto>>>
    {
        public async Task<Result<IEnumerable<RestaurantDto>>> Handle(
            GetRestaurantsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                var restaurants = await _restaurantRepository.GetRestaurantsAsync(cancellationToken);
                return Result.Success(restaurants);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                return Result.Failure<IEnumerable<RestaurantDto>>("La operación fue cancelada.");
            }
            catch (Exception ex)
            {
                return Result.Failure<IEnumerable<RestaurantDto>>(ex.Message);
            }
        }
    }
}