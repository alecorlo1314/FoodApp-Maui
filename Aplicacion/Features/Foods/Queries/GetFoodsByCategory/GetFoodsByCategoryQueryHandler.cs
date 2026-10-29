using FoodApp.Aplicacion.Common.Interfaces;
using FoodApp.Aplicacion.Features.Foods.Queries.GetFoodsByCategory;
using FoodApp.Domain.Common;
using MediatR;

namespace FoodApp.Aplicacion.Features.Foods.Queries.GetFoodsByCategory;

public class GetFoodsByCategoryQueryHandler(IFoodRepository _foodRepository) : 
    IRequestHandler<GetFoodsByCategoryQuery, Result<IEnumerable<FoodItemDto>>>
{
    public async Task<Result<IEnumerable<FoodItemDto>>> Handle(GetFoodsByCategoryQuery request, CancellationToken cancellationToken)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            var foods = await _foodRepository.GetFoodsByCategoryAsync(request.RestaurantId, request.CategoryId, cancellationToken);
            return Result.Success(foods);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            return Result.Failure<IEnumerable<FoodItemDto>>("La operación fue cancelada.");
        }
        catch (Exception ex)
        {
            return Result.Failure<IEnumerable<FoodItemDto>>(ex.Message);
        }
    }
}