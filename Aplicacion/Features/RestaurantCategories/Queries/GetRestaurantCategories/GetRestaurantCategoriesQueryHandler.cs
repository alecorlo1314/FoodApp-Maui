using FoodApp.Aplicacion.Common.Interfaces;
using FoodApp.Domain.Common;
using FoodApp.Domain.Entities;
using MediatR;

namespace FoodApp.Aplicacion.Features.RestaurantCategories.Queries.GetRestaurantCategories
{
    public class GetRestaurantCategoriesQueryHandler(IRestaurantCategoryRepository _repository) : IRequestHandler<GetRestaurantCategoriesQuery, Result<IEnumerable<RestaurantCategory>>>
    {
        public async Task<Result<IEnumerable<RestaurantCategory>>> Handle(GetRestaurantCategoriesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                var categories = await _repository.GetCategoriesByRestaurantAsync(request.RestaurantId, cancellationToken);
                return Result.Success(categories);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                return Result.Failure<IEnumerable<RestaurantCategory>>("La operación fue cancelada.");
            }
            catch (Exception ex)
            {
                return Result.Failure<IEnumerable<RestaurantCategory>>(ex.Message);
            }
        }
    }
}