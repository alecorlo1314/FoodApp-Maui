using FoodApp.Aplicacion.Common.Interfaces;
using FoodApp.Domain.Common;
using FoodApp.Domain.Entities;
using MediatR;

namespace FoodApp.Aplicacion.Features.Restaurants.Queries.GetRestaurants
{
    public class GetRestaurantsQueryHandler(IRestaurantRepository _restaurantRepository) : 
        IRequestHandler<GetRestaurantsQuery, Result<IEnumerable<Restaurant>>>
    {
        public async Task<Result<IEnumerable<Restaurant>>> Handle(GetRestaurantsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                var restaurants = await _restaurantRepository.GetRestaurantsAsync(cancellationToken);
                return Result.Success(restaurants);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                return Result.Failure<IEnumerable<Restaurant>>("La operación fue cancelada.");
            }
            catch (Exception ex)
            {
                return Result.Failure<IEnumerable<Restaurant>>(ex.Message);
            }
        }
    }
}